using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using System.Diagnostics;

namespace ProductService.Controllers
{

    public class HomeProductController : Controller
    {
        private readonly ILogger<HomeProductController> _logger;

        public HomeProductController(ILogger<HomeProductController> logger)
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
