
using Microsoft.AspNetCore.Mvc;
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
        return View(await _context.Posts.ToListAsync());
    }

    // GET: POSTS/Details/5
    public async Task<IActionResult> Details(int? id)
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

    // GET: POSTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: POSTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Content,ImageUrl,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt")] Post baiViet)
    {
        if (ModelState.IsValid)
        {
            _context.Add(baiViet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(baiViet);
    }

    // GET: POSTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var baiViet = await _context.Posts.FindAsync(id);
        if (baiViet == null)
        {
            return NotFound();
        }
        return View(baiViet);
    }

    // POST: POSTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Content,ImageUrl,Status,RejectionReason,CategoryId,Category,UserId,User,ApprovedBy,AdminApprover,ApprovedAt,CreatedAt,UpdatedAt")] Post baiViet)
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
}
