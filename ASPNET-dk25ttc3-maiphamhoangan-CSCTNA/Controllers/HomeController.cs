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
                .Include(post => post.Ratings)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt)
                .ToList();

            var kitchenCorners = _context.Posts
                .AsNoTracking()
                .Include(post => post.User)
                .Include(post => post.Ratings)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.KitchenCorner)
                .ToList();

            var authorRecipeRatings = approvedPosts
                .Where(post => post.Ratings.Any())
                .GroupBy(post => post.UserId)
                .ToDictionary(
                    group => group.Key,
                    group => new
                    {
                        AverageRating = group.SelectMany(post => post.Ratings).Average(rating => rating.Score),
                        RatingCount = group.SelectMany(post => post.Ratings).Count()
                    });

            var featuredKitchen = kitchenCorners
                .Select(post =>
                {
                    var kitchenAverageRating = post.Ratings.Any()
                        ? post.Ratings.Average(rating => rating.Score)
                        : 0;
                    var authorRating = authorRecipeRatings.TryGetValue(post.UserId, out var rating)
                        ? rating
                        : null;

                    return new
                    {
                        KitchenCorner = post,
                        KitchenAverageRating = kitchenAverageRating,
                        KitchenRatingCount = post.Ratings.Count,
                        AuthorRecipeAverageRating = authorRating?.AverageRating ?? 0,
                        AuthorRecipeRatingCount = authorRating?.RatingCount ?? 0
                    };
                })
                .OrderByDescending(item => item.KitchenAverageRating + item.AuthorRecipeAverageRating)
                .ThenByDescending(item => item.KitchenRatingCount + item.AuthorRecipeRatingCount)
                .ThenByDescending(item => item.KitchenCorner.CreatedAt)
                .FirstOrDefault();

            var featuredRecipe = featuredKitchen == null
                ? null
                : approvedPosts
                    .Where(post => post.UserId == featuredKitchen.KitchenCorner.UserId)
                    .OrderByDescending(post => post.Ratings.Any() ? post.Ratings.Average(rating => rating.Score) : 0)
                    .ThenByDescending(post => post.Ratings.Count)
                    .ThenByDescending(post => post.CreatedAt)
                    .FirstOrDefault();

            var viewModel = new HomeIndexViewModel
            {
                TasteCategories = tasteCategories,
                SuggestedPosts = approvedPosts.Take(4).ToList(),
                FeaturedKitchenCorner = featuredKitchen?.KitchenCorner,
                FeaturedRecipe = featuredRecipe,
                FeaturedKitchenAverageRating = featuredKitchen?.KitchenAverageRating ?? 0,
                FeaturedRecipeAverageRating = featuredRecipe?.Ratings.Any() == true
                    ? featuredRecipe.Ratings.Average(rating => rating.Score)
                    : 0,
                FeaturedAuthorRecipeAverageRating = featuredKitchen?.AuthorRecipeAverageRating ?? 0
            };

            return View(viewModel);
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
