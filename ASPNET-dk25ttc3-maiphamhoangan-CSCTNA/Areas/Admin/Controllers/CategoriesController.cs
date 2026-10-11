
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
    public IActionResult Index()    
    {
        return View(_context.Categories.ToList());
    }

    // GET: CATEGORYS/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = _context.Categories
            .FirstOrDefault(dong => dong.Id == id);
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
    public IActionResult Create([Bind("Id,Name,Slug,ImageUrl,Posts")] Category danhMuc)
    {
        if (ModelState.IsValid)
        {
            _context.Add(danhMuc);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(danhMuc);
    }

    // GET: CATEGORYS/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = _context.Categories.Find(id);
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
    public IActionResult Edit(int? id, [Bind("Id,Name,Slug,ImageUrl,Posts")] Category danhMuc)
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
                _context.SaveChanges();
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
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var danhMuc = _context.Categories
            .FirstOrDefault(dong => dong.Id == id);
        if (danhMuc == null)
        {
            return NotFound();
        }

        return View(danhMuc);
    }

    // POST: CATEGORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int? id)
    {
        var danhMuc = _context.Categories.Find(id);
        if (danhMuc != null)
        {
            _context.Categories.Remove(danhMuc);
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int? id)
    {
        return _context.Categories.Any(dong => dong.Id == id);
    }
}
