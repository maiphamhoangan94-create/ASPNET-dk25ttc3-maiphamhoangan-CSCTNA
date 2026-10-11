using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly AnmphDbContext _context;

        public ReportsController(AnmphDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var now = DateTime.Now;
            var sixMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

            var posts = _context.Posts.AsNoTracking();
            var ratings = _context.PostRatings.AsNoTracking();

            var recipeCountsByCategory = posts
                .Where(post => post.PostType == PostTypes.Recipe && post.CategoryId != null)
                .GroupBy(post => post.CategoryId)
                .Select(group => new
                {
                    CategoryId = group.Key,
                    Count = group.Count()
                })
                .ToList();

            var categoryNames = _context.Categories
                .AsNoTracking()
                .ToDictionary(category => category.Id, category => category.Name);

            var monthlyPosts = posts
                .Where(post => post.CreatedAt >= sixMonthsAgo)
                .GroupBy(post => new { post.CreatedAt.Year, post.CreatedAt.Month })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    Count = group.Count()
                })
                .ToList();

            var model = new AdminReportViewModel
            {
                TotalRecipes = posts.Count(post => post.PostType == PostTypes.Recipe),
                TotalKitchenCorners = posts.Count(post => post.PostType == PostTypes.KitchenCorner),
                TotalUsers = _context.Users.Count(),
                TotalCategories = _context.Categories.Count(),
                PendingPosts = posts.Count(post => post.Status == 0),
                ApprovedPosts = posts.Count(post => post.Status == 1),
                RejectedPosts = posts.Count(post => post.Status == 2),
                TotalComments = _context.PostComments.Count(),
                TotalRatings = ratings.Count(),
                AverageRating = ratings.Any() ? Math.Round(ratings.Average(rating => rating.Score), 1) : 0,
                PostsByCategory = recipeCountsByCategory
                    .Select(item => new ChartDataPoint
                    {
                        Label = item.CategoryId.HasValue && categoryNames.TryGetValue(item.CategoryId.Value, out var categoryName)
                            ? categoryName
                            : "Chưa phân loại",
                        Value = item.Count
                    })
                    .OrderByDescending(item => item.Value)
                    .Take(6)
                    .ToList(),
                PostsByStatus = new List<ChartDataPoint>
                {
                    new() { Label = "Chờ duyệt", Value = posts.Count(post => post.Status == 0) },
                    new() { Label = "Đã duyệt", Value = posts.Count(post => post.Status == 1) },
                    new() { Label = "Từ chối", Value = posts.Count(post => post.Status == 2) }
                },
                EngagementSummary = new List<ChartDataPoint>
                {
                    new() { Label = "Bình luận", Value = _context.PostComments.Count() },
                    new() { Label = "Đánh giá", Value = ratings.Count() },
                    new() { Label = "Bài đã duyệt", Value = posts.Count(post => post.Status == 1) }
                }
            };

            for (var month = sixMonthsAgo; month <= now; month = month.AddMonths(1))
            {
                var postCount = monthlyPosts.FirstOrDefault(item => item.Year == month.Year && item.Month == month.Month)?.Count ?? 0;
                model.MonthlyPosts.Add(new ChartDataPoint
                {
                    Label = month.ToString("MM/yyyy"),
                    Value = postCount
                });
            }

            return View(model);
        }
    }
}
