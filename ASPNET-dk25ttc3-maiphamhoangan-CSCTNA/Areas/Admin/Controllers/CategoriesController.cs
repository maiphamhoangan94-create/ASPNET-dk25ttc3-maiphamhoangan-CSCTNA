
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly AnmphDbContext _context;

    public CategoriesController(AnmphDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Categories.ToListAsync());
    }

    // GET: CATEGORYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = await _context.Categories
            .FirstOrDefaultAsync(dong => dong.Id == id);
        if (danhMuc == null)
        {
            return NotFound();
        }

        return View(danhMuc);
    }

    // GET: CATEGORYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CATEGORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Slug,ImageUrl,Posts")] Category danhMuc)
    {
        if (ModelState.IsValid)
        {
            _context.Add(danhMuc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(danhMuc);
    }

    // GET: CATEGORYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = await _context.Categories.FindAsync(id);
        if (danhMuc == null)
        {
            return NotFound();
        }
        return View(danhMuc);
    }

    // POST: CATEGORYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Slug,ImageUrl,Posts")] Category danhMuc)
    {
        if (id != danhMuc.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(danhMuc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(danhMuc.Id))
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
        return View(danhMuc);
    }

    // GET: CATEGORYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = await _context.Categories
            .FirstOrDefaultAsync(dong => dong.Id == id);
        if (danhMuc == null)
        {
            return NotFound();
        }

        return View(danhMuc);
    }

    // POST: CATEGORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var danhMuc = await _context.Categories.FindAsync(id);
        if (danhMuc != null)
        {
            _context.Categories.Remove(danhMuc);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int? id)
    {
        return _context.Categories.Any(dong => dong.Id == id);
    }
}
