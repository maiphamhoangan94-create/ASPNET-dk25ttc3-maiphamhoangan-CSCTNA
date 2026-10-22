using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
        public IActionResult Categories()
        {
            var categories = _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Index(int? categoryId, int page = 1)
        {
            const int pageSize = 8;
            page = Math.Max(page, 1);

            var categories = _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToList();

            IQueryable<Post> approvedRecipes = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt);

            if (categoryId.HasValue && categories.Any(category => category.Id == categoryId.Value))
            {
                approvedRecipes = approvedRecipes.Where(post => post.CategoryId == categoryId.Value);
            }
            else
            {
                categoryId = null;
            }

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
                Categories = categories,
                SelectedCategoryId = categoryId,
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

        [HttpGet]
        public IActionResult Inspiration([FromQuery] List<string>? ingredients)
        {
            var recipes = _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.User)
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .OrderByDescending(post => post.CreatedAt)
                .ToList();

            var recipeIngredients = recipes
                .Select(recipe => new
                {
                    Recipe = recipe,
                    Ingredients = ExtractIngredientNames(recipe.Content)
                })
                .Where(item => item.Ingredients.Any())
                .ToList();

            var ingredientUsage = CountIngredientUsage(recipeIngredients.Select(item => item.Ingredients));
            var availableIngredients = ingredientUsage.Select(item => item.Name).ToList();

            var selectedIngredients = (ingredients ?? new List<string>())
                .Select(ingredient => ingredient.Trim())
                .Where(ingredient => !string.IsNullOrWhiteSpace(ingredient))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var selectedKeys = selectedIngredients
                .Select(NormalizeIngredientKey)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var suggestions = selectedKeys.Any()
                ? recipeIngredients
                    .Select(item =>
                    {
                        var matchedIngredients = item.Ingredients
                            .Where(ingredient => selectedKeys.Contains(NormalizeIngredientKey(ingredient)))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        return new RecipeIngredientSuggestion
                        {
                            Recipe = item.Recipe,
                            RecipeIngredients = item.Ingredients,
                            MatchedIngredients = matchedIngredients
                        };
                    })
                    .Where(suggestion => suggestion.MatchedIngredients.Any())
                    .OrderByDescending(suggestion => suggestion.MatchedIngredients.Count)
                    .ThenBy(suggestion => suggestion.MissingIngredientCount)
                    .ThenByDescending(suggestion => suggestion.Recipe.CreatedAt)
                    .Take(12)
                    .ToList()
                : new List<RecipeIngredientSuggestion>();

            var viewModel = new IngredientSuggestionViewModel
            {
                AvailableIngredients = availableIngredients,
                SelectedIngredients = availableIngredients
                    .Where(name => selectedKeys.Contains(NormalizeIngredientKey(name))).ToList(),
                IngredientUsage = ingredientUsage,
                Suggestions = suggestions
            };

            return View(viewModel);
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
            LoadIngredientSuggestions();
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Ingredients,Content,ImageUrl,CategoryId,SelectedProvinceIds")] Post recipe)
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
                LoadIngredientSuggestions();
                return View(recipe);
            }

            recipe.UserId = userId.Value;
            recipe.PostType = PostTypes.Recipe;
            recipe.Status = 0;
            recipe.Content = BuildRecipeContent(recipe.Ingredients, recipe.Content);
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

        private void LoadIngredientSuggestions()
        {
            var contents = _context.Posts.AsNoTracking()
                .Where(post => post.Status == 1 && post.PostType == PostTypes.Recipe)
                .Select(post => post.Content).ToList();
            ViewBag.IngredientSuggestions = CountIngredientUsage(contents.Select(ExtractIngredientNames));
        }

        private static List<IngredientUsage> CountIngredientUsage(IEnumerable<List<string>> recipes)
        {
            return recipes.SelectMany(names => names
                    .Where(name => !string.IsNullOrWhiteSpace(NormalizeIngredientKey(name)))
                    .DistinctBy(NormalizeIngredientKey))
                .GroupBy(NormalizeIngredientKey)
                .Select(group => new IngredientUsage
                {
                    Name = group.GroupBy(name => name)
                        .OrderByDescending(names => names.Count())
                        .ThenBy(names => names.Key, StringComparer.Ordinal).First().Key,
                    Count = group.Count()
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase).ToList();
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

        private static string BuildRecipeContent(string? ingredients, string? instructions)
        {
            var sections = new List<string>();
            ingredients = (ingredients ?? string.Empty).Trim();
            instructions = (instructions ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(ingredients))
            {
                sections.Add($"## Nguyên liệu{Environment.NewLine}{ingredients}");
            }

            if (!string.IsNullOrWhiteSpace(instructions))
            {
                sections.Add($"## Cách làm{Environment.NewLine}{instructions}");
            }

            return string.Join($"{Environment.NewLine}{Environment.NewLine}", sections);
        }

        private static List<string> ExtractIngredientNames(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<string>();
            }

            var lines = content
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .ToList();

            var ingredients = new List<string>();
            var isIngredientSection = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("##", StringComparison.Ordinal))
                {
                    isIngredientSection = line.Contains("Nguyên liệu", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!isIngredientSection || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var ingredient = NormalizeIngredientDisplay(line);
                if (!string.IsNullOrWhiteSpace(ingredient))
                {
                    ingredients.Add(ingredient);
                }
            }

            return ingredients
                .DistinctBy(NormalizeIngredientKey)
                .ToList();
        }

        private static string NormalizeIngredientDisplay(string value)
        {
            var ingredient = value.Trim().TrimStart('-', '*', '+', '•').Trim();
            ingredient = Regex.Replace(ingredient, @"^\d+[\.\)]\s*", string.Empty);

            var dashIndex = ingredient.IndexOf(" - ", StringComparison.Ordinal);
            if (dashIndex >= 0)
            {
                ingredient = ingredient[..dashIndex];
            }

            ingredient = Regex.Replace(ingredient, @"\s*\([^()]*\)\s*$", string.Empty);
            ingredient = Regex.Replace(ingredient, @"\s+", " ").Trim();
            return ingredient;
        }

        private static string NormalizeIngredientKey(string value)
        {
            var normalized = NormalizeIngredientDisplay(value).ToLowerInvariant().Replace('đ', 'd');
            var formD = normalized.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in formD)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            return Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), @"[^a-z0-9]+", " ").Trim();
        }
    }
}
