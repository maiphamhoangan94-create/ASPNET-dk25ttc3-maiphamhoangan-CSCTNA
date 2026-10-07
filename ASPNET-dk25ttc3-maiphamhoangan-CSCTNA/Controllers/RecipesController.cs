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
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .FirstOrDefaultAsync(post => post.Id == id && post.Status == 1);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
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
