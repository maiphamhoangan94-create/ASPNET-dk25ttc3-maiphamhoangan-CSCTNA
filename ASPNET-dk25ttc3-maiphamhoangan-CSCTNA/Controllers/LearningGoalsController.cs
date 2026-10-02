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
            var summary = await _adoNetReportService.GetSummaryAsync();
            return View(summary);
        }

        [HttpGet]
        public async Task<IActionResult> CategoriesXml()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToListAsync();

            var document = new XDocument(
                new XElement("categories",
                    categories.Select(category =>
                        new XElement("category",
                            new XAttribute("id", category.Id),
                            new XElement("name", category.Name),
                            new XElement("slug", category.Slug ?? string.Empty)))));

            return Content(document.ToString(), "application/xml");
        }
    }
}
