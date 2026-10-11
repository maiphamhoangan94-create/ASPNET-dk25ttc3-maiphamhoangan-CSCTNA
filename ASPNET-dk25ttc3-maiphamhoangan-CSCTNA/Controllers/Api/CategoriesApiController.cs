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
        public IActionResult GetCategories()
        {
            var danhSachDanhMuc = _context.Categories
                .AsNoTracking()
                .OrderBy(danhMuc => danhMuc.Name)
                .Select(danhMuc => new
                {
                    danhMuc.Id,
                    danhMuc.Name,
                    danhMuc.Slug
                })
                .ToList();

            return Ok(danhSachDanhMuc);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetCategory(int id)
        {
            var danhMuc = _context.Categories
                .AsNoTracking()
                .Where(dong => dong.Id == id)
                .Select(dong => new
                {
                    dong.Id,
                    dong.Name,
                    dong.Slug
                })
                .FirstOrDefault();

            return danhMuc == null ? NotFound() : Ok(danhMuc);
        }
    }
}
