using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserMicroservice.Models;

namespace UserMicroservice.Controllers
{
    // Ovim atributom sakrij ceo kontroler iz Swagger dokumentacije
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeUserController : Controller
    {
        private readonly ILogger<HomeUserController> _logger;


        public HomeUserController(ILogger<HomeUserController> logger)
        {
            _logger = logger;
        }
        [HttpGet("index")] // Ruta za Index akciju
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("privacy")] // Ruta za privacy akciju
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("error")] // Ruta za error akciju
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
