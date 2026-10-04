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

        public async Task<IActionResult> Index()
        {
            var danhSachNguoiDung = await _context.Users
                .AsNoTracking()
                .OrderBy(nguoiDung => nguoiDung.Username)
                .ToListAsync();

            return View(danhSachNguoiDung);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(dong => dong.UserId == id);

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
        public async Task<IActionResult> Create([Bind("Username,Email,Password,FullName,Role")] User nguoiDung)
        {
            if (await IsUsernameOrEmailExistsAsync(nguoiDung.Username, nguoiDung.Email))
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
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.Users.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            nguoiDung.Password = string.Empty;
            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,Password,FullName,Role,CreatedAt")] User nguoiDung)
        {
            if (id != nguoiDung.UserId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(nguoiDung.Password))
            {
                ModelState.Remove("Password");
            }

            if (await IsUsernameOrEmailExistsAsync(nguoiDung.Username, nguoiDung.Email, nguoiDung.UserId))
            {
                ModelState.AddModelError("", "Ten dang nhap hoac email da ton tai.");
            }

            if (!ModelState.IsValid)
            {
                return View(nguoiDung);
            }

            var nguoiDungCu = await _context.Users.FindAsync(id);
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(dong => dong.UserId == id);

            return nguoiDung == null ? NotFound() : View(nguoiDung);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.Users.FindAsync(id);
            if (nguoiDung == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Users.Remove(nguoiDung);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Khong the xoa user nay vi dang co du lieu lien quan.");
                return View(nguoiDung);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsUsernameOrEmailExistsAsync(string taiKhoan, string email, int? maNguoiDungBoQua = null)
        {
            return await _context.Users.AnyAsync(nguoiDung =>
                (nguoiDung.Username == taiKhoan || nguoiDung.Email == email) &&
                (!maNguoiDungBoQua.HasValue || nguoiDung.UserId != maNguoiDungBoQua.Value));
        }

        private static string EncodePassword(string matKhau)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(matKhau));
        }
    }
}
