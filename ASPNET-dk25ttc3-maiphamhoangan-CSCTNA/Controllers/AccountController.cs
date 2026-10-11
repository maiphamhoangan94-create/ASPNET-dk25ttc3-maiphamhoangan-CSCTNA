using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers
{
    public class AccountController : Controller
    {
        private readonly AnmphDbContext _context;

        public AccountController(AnmphDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel duLieuDangKy)
        {
            if (!ModelState.IsValid)
            {
                return View(duLieuDangKy);
            }

            bool daTonTai = _context.Users.Any(nguoiDung => nguoiDung.Username == duLieuDangKy.Username || nguoiDung.Email == duLieuDangKy.Email);
            if (daTonTai)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc Email đã tồn tại!");
                return View(duLieuDangKy);
            }

            var nguoiDungMoi = new User
            {
                Username = duLieuDangKy.Username,
                Email = duLieuDangKy.Email,
                Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(duLieuDangKy.Password)),
                FullName = duLieuDangKy.FullName,
                CreatedAt = DateTime.Now,
                Role = "User"
            };

            _context.Users.Add(nguoiDungMoi);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Đăng ký tài khoản thành công!";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel duLieuDangNhap)
        {
            if (!ModelState.IsValid)
            {
                return View(duLieuDangNhap);
            }

            string matKhauDaMaHoa = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(duLieuDangNhap.Password));

            var nguoiDung = _context.Users.FirstOrDefault(nguoiDung =>
                (nguoiDung.Username == duLieuDangNhap.UsernameOrEmail || nguoiDung.Email == duLieuDangNhap.UsernameOrEmail)
                && nguoiDung.Password == matKhauDaMaHoa);

            if (nguoiDung != null)
            {
                HttpContext.Session.SetString("Username", nguoiDung.Username);
                HttpContext.Session.SetInt32("UserId", nguoiDung.UserId);
                HttpContext.Session.SetString("Role", nguoiDung.Role ?? "User");

                if (string.Equals(nguoiDung.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Categories", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên đăng nhập/Email hoặc mật khẩu không chính xác.");
            return View(duLieuDangNhap);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int? id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var profileUserId = id ?? currentUserId;

            if (profileUserId == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để xem trang cá nhân.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UserId == profileUserId.Value);

            if (user == null)
            {
                return NotFound();
            }

            var isOwner = currentUserId == user.UserId;
            var visibleStatuses = isOwner ? new[] { 0, 1, 2 } : new[] { 1 };

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(post => post.Category)
                .Include(post => post.PostProvinces)
                    .ThenInclude(postProvince => postProvince.Province)
                .Include(post => post.Comments)
                .Include(post => post.Ratings)
                .Where(post => post.UserId == user.UserId && visibleStatuses.Contains(post.Status))
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                User = user,
                IsOwner = isOwner,
                Posts = posts.Select(post => new ProfilePostViewModel
                {
                    Post = post,
                    CommentCount = post.Comments.Count,
                    RatingCount = post.Ratings.Count,
                    RatingScoreTotal = post.Ratings.Sum(rating => rating.Score)
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
