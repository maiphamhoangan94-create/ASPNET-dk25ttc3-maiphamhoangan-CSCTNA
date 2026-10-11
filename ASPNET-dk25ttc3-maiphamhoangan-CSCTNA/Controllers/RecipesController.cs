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
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 8;
            page = Math.Max(page, 1);

            var approvedRecipes = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1)
                .OrderByDescending(post => post.CreatedAt);

            var totalRecipes = await approvedRecipes.CountAsync();
            var totalPages = Math.Max((int)Math.Ceiling(totalRecipes / (double)pageSize), 1);

            if (page > totalPages)
            {
                page = totalPages;
            }

            var recipes = await approvedRecipes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Include(post => post.Comments.OrderBy(comment => comment.CreatedAt))
                    .ThenInclude(comment => comment.User)
                .FirstOrDefaultAsync(post => post.Id == id && post.Status == 1);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content, int? parentCommentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để bình luận.";
                return RedirectToAction("Login", "Account");
            }

            var recipeExists = await _context.Posts
                .AnyAsync(post => post.Id == postId && post.Status == 1);

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
                var parentComment = await _context.PostComments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(comment => comment.Id == parentCommentId.Value && comment.PostId == postId);

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

            await _context.SaveChangesAsync();
            TempData["CommentSuccess"] = parentCommentId.HasValue
                ? "Đã gửi phản hồi của bạn."
                : "Đã gửi bình luận của bạn.";

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đăng công thức nấu ăn.";
                return RedirectToAction("Login", "Account");
            }

            await LoadCategoriesAsync();
            await LoadProvincesAsync();
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,ImageUrl,CategoryId,SelectedProvinceIds")] Post recipe)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đăng công thức nấu ăn.";
                return RedirectToAction("Login", "Account");
            }

            if (!await _context.Categories.AnyAsync(category => category.Id == recipe.CategoryId))
            {
                ModelState.AddModelError(nameof(Post.CategoryId), "Danh mục không hợp lệ.");
            }

            if (recipe.SelectedProvinceIds.Count > 0)
            {
                var provinceCount = await _context.Provinces
                    .CountAsync(province => recipe.SelectedProvinceIds.Contains(province.Id));

                if (provinceCount != recipe.SelectedProvinceIds.Distinct().Count())
                {
                    ModelState.AddModelError(nameof(Post.SelectedProvinceIds), "Tỉnh thành không hợp lệ.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(recipe.CategoryId);
                await LoadProvincesAsync(recipe.SelectedProvinceIds);
                return View(recipe);
            }

            recipe.UserId = userId.Value;
            recipe.Status = 0;
            recipe.CreatedAt = DateTime.Now;
            recipe.UpdatedAt = null;
            recipe.ApprovedBy = null;
            recipe.ApprovedAt = null;
            recipe.RejectionReason = null;

            _context.Posts.Add(recipe);
            await _context.SaveChangesAsync();

            await SavePostProvincesAsync(recipe.Id, recipe.SelectedProvinceIds);

            TempData["RecipeCreatedMessage"] = "Công thức của bạn đã được gửi và đang chờ duyệt.";
            return RedirectToAction(nameof(Create));
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private async Task LoadCategoriesAsync(int? selectedCategoryId = null)
        {
            var categories = await _context.Categories
                .OrderBy(category => category.Name)
                .ToListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }

        private async Task LoadProvincesAsync(IEnumerable<int>? selectedProvinceIds = null)
        {
            var provinces = await _context.Provinces
                .OrderBy(province => province.Name)
                .ToListAsync();

            ViewBag.Provinces = new MultiSelectList(provinces, "Id", "Name", selectedProvinceIds);
        }

        private async Task SavePostProvincesAsync(int postId, IEnumerable<int> provinceIds)
        {
            var postProvinces = provinceIds
                .Distinct()
                .Select(provinceId => new PostProvince
                {
                    PostId = postId,
                    ProvinceId = provinceId
                });

            _context.PostProvinces.AddRange(postProvinces);
            await _context.SaveChangesAsync();
        }
    }
}
