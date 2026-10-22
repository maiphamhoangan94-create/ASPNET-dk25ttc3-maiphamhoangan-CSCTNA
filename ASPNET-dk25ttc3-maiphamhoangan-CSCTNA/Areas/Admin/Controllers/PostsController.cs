using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

[Area("Admin")]
public class PostsController : Controller
{
    private readonly AnmphDbContext _context;
    private readonly ImageUploadService _imageUpload;

    public PostsController(AnmphDbContext context, ImageUploadService imageUpload)
    {
        _context = context;
        _imageUpload = imageUpload;
    }

    // GET: POSTS
    public IActionResult Index()    
    {
        return View(_context.Posts
            .Include(baiViet => baiViet.Category)
            .Include(baiViet => baiViet.User)
            .Include(baiViet => baiViet.PostProvinces)
                .ThenInclude(postProvince => postProvince.Province)
            .Where(baiViet => baiViet.PostType == PostTypes.Recipe)
            .OrderByDescending(baiViet => baiViet.CreatedAt)
            .ToList());
    }

    // GET: POSTS/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = _context.Posts
            .Include(dong => dong.Category)
            .Include(dong => dong.User)
            .Include(dong => dong.AdminApprover)
            .Include(dong => dong.PostProvinces)
                .ThenInclude(postProvince => postProvince.Province)
            .FirstOrDefault(dong => dong.Id == id && dong.PostType == PostTypes.Recipe);
        if (baiViet == null)
        {
            return NotFound();
        }

        return View(baiViet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int id)
    {
        var baiViet = _context.Posts.Find(id);
        if (baiViet == null || baiViet.PostType != PostTypes.Recipe)
        {
            return NotFound();
        }

        baiViet.Status = 1;
        baiViet.ApprovedBy = HttpContext.Session.GetInt32("UserId");
        baiViet.ApprovedAt = DateTime.Now;
        baiViet.RejectionReason = null;
        baiViet.UpdatedAt = DateTime.Now;

        _context.SaveChanges();
        TempData["PostMessage"] = "Bài viết đã được phê duyệt.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string? rejectionReason)
    {
        var baiViet = _context.Posts.Find(id);
        if (baiViet == null || baiViet.PostType != PostTypes.Recipe)
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

        _context.SaveChanges();
        TempData["PostMessage"] = "Bài viết đã bị từ chối.";

        return RedirectToAction(nameof(Index));
    }

    // GET: POSTS/Create
    public IActionResult Create()
    {
        LoadProvinces();
        return View();
    }

    // POST: POSTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Title,Content,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt,SelectedProvinceIds")] Post baiViet, IFormFile? imageFile)
    {
        _imageUpload.Validate(imageFile, ModelState);

        if (ModelState.IsValid)
        {
            if (imageFile != null) baiViet.ImageUrl = _imageUpload.Save(imageFile, "recipes");
            baiViet.PostType = PostTypes.Recipe;
            _context.Add(baiViet);
            _context.SaveChanges();
            SavePostProvinces(baiViet.Id, baiViet.SelectedProvinceIds);
            return RedirectToAction(nameof(Index));
        }
        LoadProvinces(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // GET: POSTS/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = _context.Posts
            .Include(post => post.PostProvinces)
            .FirstOrDefault(post => post.Id == id && post.PostType == PostTypes.Recipe);
        if (baiViet == null)
        {
            return NotFound();
        }
        baiViet.SelectedProvinceIds = baiViet.PostProvinces
            .Select(postProvince => postProvince.ProvinceId)
            .ToList();
        LoadProvinces(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // POST: POSTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int? id, [Bind("Id,Title,Content,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt,SelectedProvinceIds")] Post baiViet, IFormFile? imageFile)
    {
        if (id != baiViet.Id)
        {
            return NotFound();
        }

        _imageUpload.Validate(imageFile, ModelState);
        var existing = _context.Posts.AsNoTracking().FirstOrDefault(item => item.Id == baiViet.Id);
        if (existing == null) return NotFound();
        baiViet.ImageUrl = existing.ImageUrl;

        if (ModelState.IsValid)
        {
            if (imageFile != null) baiViet.ImageUrl = _imageUpload.Save(imageFile, "recipes");
            try
            {
                baiViet.PostType = PostTypes.Recipe;
                _context.Update(baiViet);
                _context.SaveChanges();
                SavePostProvinces(baiViet.Id, baiViet.SelectedProvinceIds);
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
        LoadProvinces(baiViet.SelectedProvinceIds);
        return View(baiViet);
    }

    // GET: POSTS/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = _context.Posts
            .FirstOrDefault(dong => dong.Id == id && dong.PostType == PostTypes.Recipe);
        if (baiViet == null)
        {
            return NotFound();
        }

        return View(baiViet);
    }

    // POST: POSTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int? id)
    {
        var baiViet = _context.Posts.Find(id);
        if (baiViet != null)
        {
            if (baiViet.PostType != PostTypes.Recipe)
            {
                return NotFound();
            }

            _context.Posts.Remove(baiViet);
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private bool PostExists(int? id)
    {
        return _context.Posts.Any(dong => dong.Id == id);
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
        var currentPostProvinces = _context.PostProvinces
            .Where(postProvince => postProvince.PostId == postId)
            .ToList();

        _context.PostProvinces.RemoveRange(currentPostProvinces);

        var nextPostProvinces = provinceIds
            .Distinct()
            .Select(provinceId => new PostProvince
            {
                PostId = postId,
                ProvinceId = provinceId
            });

        _context.PostProvinces.AddRange(nextPostProvinces);
        _context.SaveChanges();
    }
}
