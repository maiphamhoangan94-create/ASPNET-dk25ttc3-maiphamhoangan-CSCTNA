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
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(user => user.Username)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UserId == id);

            return user == null ? NotFound() : View(user);
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
        public async Task<IActionResult> Create([Bind("Username,Email,Password,FullName,Role")] User user)
        {
            if (await IsUsernameOrEmailExistsAsync(user.Username, user.Email))
            {
                ModelState.AddModelError("", "Ten dang nhap hoac email da ton tai.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Password = EncodePassword(user.Password);
            user.CreatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = string.Empty;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,Password,FullName,Role,CreatedAt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.Remove("Password");
            }

            if (await IsUsernameOrEmailExistsAsync(user.Username, user.Email, user.UserId))
            {
                ModelState.AddModelError("", "Ten dang nhap hoac email da ton tai.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.Role = user.Role;
            existingUser.CreatedAt = user.CreatedAt;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = EncodePassword(user.Password);
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

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UserId == id);

            return user == null ? NotFound() : View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Khong the xoa user nay vi dang co du lieu lien quan.");
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsUsernameOrEmailExistsAsync(string username, string email, int? ignoreUserId = null)
        {
            return await _context.Users.AnyAsync(user =>
                (user.Username == username || user.Email == email) &&
                (!ignoreUserId.HasValue || user.UserId != ignoreUserId.Value));
        }

        private static string EncodePassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
