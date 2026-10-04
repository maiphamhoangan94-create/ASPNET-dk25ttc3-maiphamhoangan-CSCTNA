
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

[Area("Admin")]
public class PostsController : Controller
{
    private readonly AnmphDbContext _context;

    public PostsController(AnmphDbContext context)
    {
        _context = context;
    }

    // GET: POSTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Posts
            .Include(baiViet => baiViet.Category)
            .Include(baiViet => baiViet.User)
            .Include(baiViet => baiViet.PostProvinces)
                .ThenInclude(postProvince => postProvince.Province)
            .OrderByDescending(baiViet => baiViet.CreatedAt)
            .ToListAsync());
    }

    // GET: POSTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = await _context.Posts
            .Include(dong => dong.Category)
            .Include(dong => dong.User)
            .Include(dong => dong.AdminApprover)
            .Include(dong => dong.PostProvinces)
                .ThenInclude(postProvince => postProvince.Province)
            .FirstOrDefaultAsync(dong => dong.Id == id);
        if (baiViet == null)
        {
            return NotFound();
        }

        return View(baiViet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var baiViet = await _context.Posts.FindAsync(id);
        if (baiViet == null)
        {
            return NotFound();
        }

        baiViet.Status = 1;
        baiViet.ApprovedBy = HttpContext.Session.GetInt32("UserId");
        baiViet.ApprovedAt = DateTime.Now;
        baiViet.RejectionReason = null;
        baiViet.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        TempData["PostMessage"] = "Bài viết đã được phê duyệt.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string? rejectionReason)
    {
        var baiViet = await _context.Posts.FindAsync(id);
        if (baiViet == null)
        {
            return NotFound();
        }

        baiViet.Status = 2;
        baiViet.ApprovedBy = HttpContext.Session.GetInt32("UserId");
        baiViet.ApprovedAt = DateTime.Now;
        baiViet.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason)
            ? "Không đạt yêu cầu duyệt nội dung."
            : rejectionReason.Trim();
        baiViet.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        TempData["PostMessage"] = "Bài viết đã bị từ chối.";

        return RedirectToAction(nameof(Index));
    }

    // GET: POSTS/Create
    public async Task<IActionResult> Create()
    {
        await LoadProvincesAsync();
        return View();
    }

    // POST: POSTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Content,ImageUrl,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt,SelectedProvinceIds")] Post baiViet)
    {
        if (ModelState.IsValid)
        {
            _context.Add(baiViet);
            await _context.SaveChangesAsync();
            await SavePostProvincesAsync(baiViet.Id, baiViet.SelectedProvinceIds);
            return RedirectToAction(nameof(Index));
        }
        await LoadProvincesAsync(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // GET: POSTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = await _context.Posts
            .Include(post => post.PostProvinces)
            .FirstOrDefaultAsync(post => post.Id == id);
        if (baiViet == null)
        {
            return NotFound();
        }
        baiViet.SelectedProvinceIds = baiViet.PostProvinces
            .Select(postProvince => postProvince.ProvinceId)
            .ToList();
        await LoadProvincesAsync(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // POST: POSTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Content,ImageUrl,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt,SelectedProvinceIds")] Post baiViet)
    {
        if (id != baiViet.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(baiViet);
                await _context.SaveChangesAsync();
                await SavePostProvincesAsync(baiViet.Id, baiViet.SelectedProvinceIds);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(baiViet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        await LoadProvincesAsync(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // GET: POSTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = await _context.Posts
            .FirstOrDefaultAsync(dong => dong.Id == id);
        if (baiViet == null)
        {
            return NotFound();
        }

        return View(baiViet);
    }

    // POST: POSTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var baiViet = await _context.Posts.FindAsync(id);
        if (baiViet != null)
        {
            _context.Posts.Remove(baiViet);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PostExists(int? id)
    {
        return _context.Posts.Any(dong => dong.Id == id);
    }

    private async Task LoadProvincesAsync(IEnumerable<int>? selectedProvinceIds = null)
    {
        var provinces = await _context.Provinces
            .OrderBy(province => province.Name)
            .ToListAsync();

        ViewBag.Provinces = new MultiSelectList(provinces, "Id", "Name", selectedProvinceIds);
    }

    private async Task SavePostProvincesAsync(int postId, IEnumerable<int> provinceIds)
    {
        var currentPostProvinces = await _context.PostProvinces
            .Where(postProvince => postProvince.PostId == postId)
            .ToListAsync();

        _context.PostProvinces.RemoveRange(currentPostProvinces);

        var nextPostProvinces = provinceIds
            .Distinct()
            .Select(provinceId => new PostProvince
            {
                PostId = postId,
                ProvinceId = provinceId
            });

        _context.PostProvinces.AddRange(nextPostProvinces);
        await _context.SaveChangesAsync();
    }
}
