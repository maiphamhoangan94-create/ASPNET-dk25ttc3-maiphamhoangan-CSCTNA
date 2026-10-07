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

        public async Task<IActionResult> Index()
        {
            var tasteCategories = await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Take(5)
                .ToListAsync();

            var approvedPosts = await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1)
                .OrderByDescending(post => post.CreatedAt)
                .Take(8)
                .ToListAsync();

            ViewBag.TasteCategories = tasteCategories;

            return View(approvedPosts);
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
