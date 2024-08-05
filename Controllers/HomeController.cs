using BuildingShopCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace BuildingShopCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BuildingShopContext _context;
        public HomeController(BuildingShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<JsonResult> GetChartData()
        {
            var orders = await _context.Orders
                .GroupBy(o => new
                {
                    Year = o.Date.Year,
                    Month = o.Date.Month
                })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    sum = g.Sum(o => o.Sum)
                })
                .OrderBy(o => o.year)
                .ToListAsync();

            return Json(orders);
        }
    }
}
