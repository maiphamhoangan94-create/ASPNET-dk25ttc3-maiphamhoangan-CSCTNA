using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly AnmphDbContext _context;
    private readonly ImageUploadService _imageUpload;

    public CategoriesController(AnmphDbContext context, ImageUploadService imageUpload)
    {
        _context = context;
        _imageUpload = imageUpload;
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
    public IActionResult Create([Bind("Id,Name,Slug,Posts")] Category danhMuc, IFormFile? imageFile)
    {
        _imageUpload.Validate(imageFile, ModelState);

        if (ModelState.IsValid)
        {
            if (imageFile != null) danhMuc.ImageUrl = _imageUpload.Save(imageFile, "categories");
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
    public IActionResult Edit(int? id, [Bind("Id,Name,Slug,Posts")] Category danhMuc, IFormFile? imageFile)
    {
        if (id != danhMuc.Id)
        {
            return NotFound();
        }

        _imageUpload.Validate(imageFile, ModelState);
        var existing = _context.Categories.AsNoTracking().FirstOrDefault(item => item.Id == danhMuc.Id);
        if (existing == null) return NotFound();
        danhMuc.ImageUrl = existing.ImageUrl;

        if (ModelState.IsValid)
        {
            if (imageFile != null) danhMuc.ImageUrl = _imageUpload.Save(imageFile, "categories");
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
