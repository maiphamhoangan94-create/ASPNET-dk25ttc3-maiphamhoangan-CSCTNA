using Microsoft.AspNetCore.Mvc;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
