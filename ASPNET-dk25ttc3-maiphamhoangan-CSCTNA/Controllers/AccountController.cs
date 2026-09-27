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
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isExist = _context.Users.Any(u => u.Username == model.Username || u.Email == model.Email);
            if (isExist)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc Email đã tồn tại!");
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Password)),
                FullName = model.FullName,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Đăng ký tài khoản thành công!";
            return View();
        }
    }
}
