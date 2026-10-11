using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KitchenCornersController : Controller
    {
        private readonly AnmphDbContext _context;

        public KitchenCornersController(AnmphDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts
                .AsNoTracking()
                .Include(post => post.User)
                .Include(post => post.Ratings)
                .Where(post => post.PostType == PostTypes.KitchenCorner)
                .OrderBy(post => post.Status == 0 ? 0 : 1)
                .ThenByDescending(post => post.CreatedAt)
                .ToList();

            return View(posts);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _context.Posts
                .AsNoTracking()
                .Include(item => item.User)
                .Include(item => item.AdminApprover)
                .Include(item => item.Ratings)
                .FirstOrDefault(item => item.Id == id && item.PostType == PostTypes.KitchenCorner);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null || post.PostType != PostTypes.KitchenCorner)
            {
                return NotFound();
            }

            post.Status = 1;
            post.ApprovedBy = HttpContext.Session.GetInt32("UserId");
            post.ApprovedAt = DateTime.Now;
            post.RejectionReason = null;
            post.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["KitchenCornerMessage"] = "Góc bếp đã được phê duyệt.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string? rejectionReason)
        {
            var post = _context.Posts.Find(id);
            if (post == null || post.PostType != PostTypes.KitchenCorner)
            {
                return NotFound();
            }

            post.Status = 2;
            post.ApprovedBy = HttpContext.Session.GetInt32("UserId");
            post.ApprovedAt = DateTime.Now;
            post.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason)
                ? "Không đạt yêu cầu duyệt nội dung."
                : rejectionReason.Trim();
            post.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            TempData["KitchenCornerMessage"] = "Góc bếp đã bị từ chối.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null || post.PostType != PostTypes.KitchenCorner)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            _context.SaveChanges();
            TempData["KitchenCornerMessage"] = "Góc bếp đã được xóa.";

            return RedirectToAction(nameof(Index));
        }
    }
}
