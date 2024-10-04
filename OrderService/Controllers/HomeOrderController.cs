using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using System.Diagnostics;

namespace OrderService.Controllers
{

    public class HomeOrderController : Controller
    {
        private readonly ILogger<HomeOrderController> _logger;

        public HomeOrderController(ILogger<HomeOrderController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
