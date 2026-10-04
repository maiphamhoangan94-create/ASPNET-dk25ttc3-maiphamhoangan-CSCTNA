using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers.Api
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly AnmphDbContext _context;

        public CategoriesApiController(AnmphDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var danhSachDanhMuc = await _context.Categories
                .AsNoTracking()
                .OrderBy(danhMuc => danhMuc.Name)
                .Select(danhMuc => new
                {
                    danhMuc.Id,
                    danhMuc.Name,
                    danhMuc.Slug
                })
                .ToListAsync();

            return Ok(danhSachDanhMuc);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var danhMuc = await _context.Categories
                .AsNoTracking()
                .Where(dong => dong.Id == id)
                .Select(dong => new
                {
                    dong.Id,
                    dong.Name,
                    dong.Slug
                })
                .FirstOrDefaultAsync();

            return danhMuc == null ? NotFound() : Ok(danhMuc);
        }
    }
}
