using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers
{
    public class RecipesController : Controller
    {
        private readonly AnmphDbContext _context;

        public RecipesController(AnmphDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 8;
            page = Math.Max(page, 1);

            var approvedRecipes = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt);

            var totalRecipes = approvedRecipes.Count();
            var totalPages = Math.Max((int)Math.Ceiling(totalRecipes / (double)pageSize), 1);

            if (page > totalPages)
            {
                page = totalPages;
            }

            var recipes = approvedRecipes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new RecipeListViewModel
            {
                Recipes = recipes,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRecipes = totalRecipes,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var recipe = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Include(post => post.Comments.OrderBy(comment => comment.CreatedAt))
                    .ThenInclude(comment => comment.User)
                .Include(post => post.Ratings)
                .FirstOrDefault(post => post.Id == id && post.Status == 1 && post.PostType == PostTypes.Recipe);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rate(int postId, int score)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đánh giá công thức.";
                return RedirectToAction("Login", "Account");
            }

            if (score < 1 || score > 5)
            {
                TempData["RatingError"] = "Vui lòng chọn điểm đánh giá từ 1 đến 5 sao.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            var recipeExists = _context.Posts
                .Any(post => post.Id == postId && post.Status == 1);

            if (!recipeExists)
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
            TempData["RatingSuccess"] = "Cảm ơn bạn đã đánh giá công thức.";

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int postId, string content, int? parentCommentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToAction("Login", "Account");
            }

            var recipeExists = _context.Posts
                .Any(post => post.Id == postId && post.Status == 1);

            if (!recipeExists)
            {
                return NotFound();
            }

            content = (content ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "Vui lòng nhập nội dung bình luận.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            if (content.Length > 1000)
            {
                TempData["CommentError"] = "Bình luận không vượt quá 1000 ký tự.";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            if (parentCommentId.HasValue)
            {
                var parentComment = _context.PostComments
                    .AsNoTracking()
                    .FirstOrDefault(comment => comment.Id == parentCommentId.Value && comment.PostId == postId);

                if (parentComment == null)
                {
                    TempData["CommentError"] = "Bình luận bạn muốn trả lời không tồn tại.";
                    return RedirectToAction(nameof(Details), new { id = postId });
                }

                if (parentComment.ParentCommentId.HasValue)
                {
                    parentCommentId = parentComment.ParentCommentId;
                }
            }

            _context.PostComments.Add(new PostComment
            {
                PostId = postId,
                UserId = userId.Value,
                ParentCommentId = parentCommentId,
                Content = content,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();
            TempData["CommentSuccess"] = parentCommentId.HasValue
                ? "Đã gửi phản hồi của bạn."
                : "Đã gửi bình luận của bạn.";

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đăng công thức nấu ăn.";
                return RedirectToAction("Login", "Account");
            }

            LoadCategories();
            LoadProvinces();
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Content,ImageUrl,CategoryId,SelectedProvinceIds")] Post recipe)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đăng công thức nấu ăn.";
                return RedirectToAction("Login", "Account");
            }

            if (!recipe.CategoryId.HasValue || !_context.Categories.Any(category => category.Id == recipe.CategoryId.Value))
            {
                ModelState.AddModelError(nameof(Post.CategoryId), "Danh mục không hợp lệ.");
            }

            if (recipe.SelectedProvinceIds.Count > 0)
            {
                var provinceCount = _context.Provinces
                    .Count(province => recipe.SelectedProvinceIds.Contains(province.Id));

                if (provinceCount != recipe.SelectedProvinceIds.Distinct().Count())
                {
                    ModelState.AddModelError(nameof(Post.SelectedProvinceIds), "Tỉnh thành không hợp lệ.");
                }
            }

            if (!ModelState.IsValid)
            {
                LoadCategories(recipe.CategoryId);
                LoadProvinces(recipe.SelectedProvinceIds);
                return View(recipe);
            }

            recipe.UserId = userId.Value;
            recipe.PostType = PostTypes.Recipe;
            recipe.Status = 0;
            recipe.CreatedAt = DateTime.Now;
            recipe.UpdatedAt = null;
            recipe.ApprovedBy = null;
            recipe.ApprovedAt = null;
            recipe.RejectionReason = null;

            _context.Posts.Add(recipe);
            _context.SaveChanges();

            SavePostProvinces(recipe.Id, recipe.SelectedProvinceIds);

            TempData["RecipeCreatedMessage"] = "Công thức của bạn đã được gửi và đang chờ duyệt.";
            return RedirectToAction(nameof(Create));
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private void LoadCategories(int? selectedCategoryId = null)
        {
            var categories = _context.Categories
                .OrderBy(category => category.Name)
                .ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }

        private void LoadProvinces(IEnumerable<int>? selectedProvinceIds = null)
        {
            var provinces = _context.Provinces
                .OrderBy(province => province.Name)
                .ToList();

            ViewBag.Provinces = new MultiSelectList(provinces, "Id", "Name", selectedProvinceIds);
        }

        private void SavePostProvinces(int postId, IEnumerable<int> provinceIds)
        {
            var postProvinces = provinceIds
                .Distinct()
                .Select(provinceId => new PostProvince
                {
                    PostId = postId,
                    ProvinceId = provinceId
                });

            _context.PostProvinces.AddRange(postProvinces);
            _context.SaveChanges();
        }
    }
}
