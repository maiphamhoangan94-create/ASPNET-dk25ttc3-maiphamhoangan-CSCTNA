using System.Xml.Linq;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Models;
using ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_dk25ttc3_maiphamhoangan_CSCTNA.Controllers
{
    public class LearningGoalsController : Controller
    {
        private readonly AnmphDbContext _context;
        private readonly AdoNetReportService _adoNetReportService;

        public LearningGoalsController(AnmphDbContext context, AdoNetReportService adoNetReportService)
        {
            _context = context;
            _adoNetReportService = adoNetReportService;
        }

        public async Task<IActionResult> Index()
        {
            var baoCao = await _adoNetReportService.GetSummaryAsync();
            return View(baoCao);
        }

        [HttpGet]
        public async Task<IActionResult> CategoriesXml()
        {
            var danhSachDanhMuc = await _context.Categories
                .AsNoTracking()
                .OrderBy(danhMuc => danhMuc.Name)
                .ToListAsync();

            var taiLieuXml = new XDocument(
                new XElement("categories",
                    danhSachDanhMuc.Select(danhMuc =>
                        new XElement("category",
                            new XAttribute("id", danhMuc.Id),
                            new XElement("name", danhMuc.Name),
                            new XElement("slug", danhMuc.Slug ?? string.Empty)))));

            return Content(taiLieuXml.ToString(), "application/xml");
        }
    }
}
