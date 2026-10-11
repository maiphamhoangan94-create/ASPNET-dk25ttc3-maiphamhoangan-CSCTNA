using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnmphDbContext _context;

        public HomeController(AnmphDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tasteCategories = _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Take(5)
                .ToList();

            var approvedPosts = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt)
                .Take(8)
                .ToList();

            ViewBag.TasteCategories = tasteCategories;

            return View(approvedPosts);
        }

        [HttpGet]
        public IActionResult ExploreProvinces(int? provinceId)
        {
            var provinces = _context.Provinces
                .AsNoTracking()
                .OrderBy(province => province.Name)
                .ToList();

            var approvedPosts = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt)
                .ToList();

            if (provinceId.HasValue && !provinces.Any(province => province.Id == provinceId.Value))
            {
                provinceId = null;
            }

            var provinceGroups = provinces
                .Select(province => new ProvinceFoodGroup
                {
                    Province = province,
                    Posts = approvedPosts
                        .Where(post => post.PostProvinces.Any(postProvince => postProvince.ProvinceId == province.Id))
                        .Take(6)
                        .ToList()
                })
                .Where(group => !provinceId.HasValue || group.Province.Id == provinceId.Value)
                .ToList();

            var viewModel = new ProvinceExploreViewModel
            {
                ProvinceGroups = provinceGroups,
                SelectedProvinceId = provinceId,
                SelectedProvinceName = provinceGroups.FirstOrDefault()?.Province.Name,
                TotalProvinceCount = provinces.Count,
                TotalRecipeCount = approvedPosts.Count(post => post.PostProvinces.Any())
            };

            ViewBag.Provinces = provinces;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
