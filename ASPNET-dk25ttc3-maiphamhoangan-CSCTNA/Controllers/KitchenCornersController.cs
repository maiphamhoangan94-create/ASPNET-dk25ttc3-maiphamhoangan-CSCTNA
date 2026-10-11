using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers
{
    public class KitchenCornersController : Controller
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        private readonly AnmphDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public KitchenCornersController(AnmphDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var posts = _context.Posts
                .AsNoTracking()
                .Include(post => post.User)
                .Include(post => post.Ratings)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.KitchenCorner)
                .OrderByDescending(post => post.Ratings.Any() ? post.Ratings.Average(rating => rating.Score) : 0)
                .ThenByDescending(post => post.Ratings.Count)
                .ThenByDescending(post => post.CreatedAt)
                .ToList();

            return View(posts);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var post = _context.Posts
                .AsNoTracking()
                .Include(item => item.User)
                .Include(item => item.Ratings)
                .FirstOrDefault(item => item.Id == id && item.Status == 1 && item.PostType == PostTypes.KitchenCorner);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để chia sẻ góc bếp.";
                return RedirectToAction("Login", "Account");
            }

            return View(new KitchenCornerCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KitchenCornerCreateViewModel viewModel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để chia sẻ góc bếp.";
                return RedirectToAction("Login", "Account");
            }

            var images = viewModel.Images
                .Where(image => image != null && image.Length > 0)
                .ToList();

            ValidateImages(images);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var imageUrls = SaveImages(images);
            var post = new Post
            {
                Title = viewModel.Title,
                Content = viewModel.Content,
                ImageUrl = imageUrls.FirstOrDefault(),
                GalleryImageUrls = string.Join(Environment.NewLine, imageUrls),
                PostType = PostTypes.KitchenCorner,
                Status = 0,
                CategoryId = null,
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            _context.SaveChanges();

            TempData["KitchenCornerCreatedMessage"] = "Góc bếp của bạn đã được gửi và đang chờ duyệt.";
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rate(int postId, int score)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đánh giá góc bếp.";
                return RedirectToAction("Login", "Account");
            }

            if (score < 1 || score > 5)
            {
                TempData["RatingError"] = "Vui lòng chọn điểm đánh giá từ 1 đến 5 sao.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            var postExists = _context.Posts
                .Any(post => post.Id == postId && post.Status == 1 && post.PostType == PostTypes.KitchenCorner);

            if (!postExists)
            {
                return NotFound();
            }

            var rating = _context.PostRatings
                .FirstOrDefault(item => item.PostId == postId && item.UserId == userId.Value);

            if (rating == null)
            {
                _context.PostRatings.Add(new PostRating
                {
                    PostId = postId,
                    UserId = userId.Value,
                    Score = score,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                rating.Score = score;
                rating.UpdatedAt = DateTime.Now;
            }

            _context.SaveChanges();
            TempData["RatingSuccess"] = "Cảm ơn bạn đã đánh giá góc bếp.";

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private void ValidateImages(IReadOnlyCollection<IFormFile> images)
        {
            if (images.Count < 1 || images.Count > 5)
            {
                ModelState.AddModelError(nameof(KitchenCornerCreateViewModel.Images), "Vui lòng tải lên từ 1 đến 5 hình ảnh.");
                return;
            }

            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName);
                if (!AllowedImageExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(KitchenCornerCreateViewModel.Images), "Chỉ hỗ trợ ảnh JPG, PNG, WEBP hoặc GIF.");
                }
            }
        }

        private List<string> SaveImages(IEnumerable<IFormFile> images)
        {
            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "kitchen-corners");
            Directory.CreateDirectory(uploadFolder);

            var imageUrls = new List<string>();
            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using var stream = System.IO.File.Create(filePath);
                image.CopyTo(stream);

                imageUrls.Add($"/uploads/kitchen-corners/{fileName}");
            }

            return imageUrls;
        }

    }
}
