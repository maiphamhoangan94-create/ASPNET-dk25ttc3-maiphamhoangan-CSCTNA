using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AnmphDbContext _context;

        public UsersController(AnmphDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var danhSachNguoiDung = _context.Users
                .AsNoTracking()
                .OrderBy(nguoiDung => nguoiDung.Username)
                .ToList();

            return View(danhSachNguoiDung);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = _context.Users
                .AsNoTracking()
                .FirstOrDefault(dong => dong.UserId == id);

            return nguoiDung == null ? NotFound() : View(nguoiDung);
        }

        public IActionResult Create()
        {
            return View(new User
            {
                CreatedAt = DateTime.Now,
                Role = "User"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Username,Email,Password,FullName,Role")] User nguoiDung)
        {
            if (IsUsernameOrEmailExists(nguoiDung.Username, nguoiDung.Email))
            {
                ModelState.AddModelError("", "Ten dang nhap hoac email da ton tai.");
            }

            if (!ModelState.IsValid)
            {
                return View(nguoiDung);
            }

            nguoiDung.Password = EncodePassword(nguoiDung.Password);
            nguoiDung.CreatedAt = DateTime.Now;

            _context.Users.Add(nguoiDung);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = _context.Users.Find(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            nguoiDung.Password = string.Empty;
            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("UserId,Username,Email,Password,FullName,Role,CreatedAt")] User nguoiDung)
        {
            if (id != nguoiDung.UserId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(nguoiDung.Password))
            {
                ModelState.Remove("Password");
            }

            if (IsUsernameOrEmailExists(nguoiDung.Username, nguoiDung.Email, nguoiDung.UserId))
            {
                ModelState.AddModelError("", "Ten dang nhap hoac email da ton tai.");
            }

            if (!ModelState.IsValid)
            {
                return View(nguoiDung);
            }

            var nguoiDungCu = _context.Users.Find(id);
            if (nguoiDungCu == null)
            {
                return NotFound();
            }

            nguoiDungCu.Username = nguoiDung.Username;
            nguoiDungCu.Email = nguoiDung.Email;
            nguoiDungCu.FullName = nguoiDung.FullName;
            nguoiDungCu.Role = nguoiDung.Role;
            nguoiDungCu.CreatedAt = nguoiDung.CreatedAt;

            if (!string.IsNullOrWhiteSpace(nguoiDung.Password))
            {
                nguoiDungCu.Password = EncodePassword(nguoiDung.Password);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = _context.Users
                .AsNoTracking()
                .FirstOrDefault(dong => dong.UserId == id);

            return nguoiDung == null ? NotFound() : View(nguoiDung);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var nguoiDung = _context.Users.Find(id);
            if (nguoiDung == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Users.Remove(nguoiDung);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Khong the xoa user nay vi dang co du lieu lien quan.");
                return View(nguoiDung);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsUsernameOrEmailExists(string taiKhoan, string email, int? maNguoiDungBoQua = null)
        {
            return _context.Users.Any(nguoiDung =>
                (nguoiDung.Username == taiKhoan || nguoiDung.Email == email) &&
                (!maNguoiDungBoQua.HasValue || nguoiDung.UserId != maNguoiDungBoQua.Value));
        }

        private static string EncodePassword(string matKhau)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(matKhau));
        }
    }
}
