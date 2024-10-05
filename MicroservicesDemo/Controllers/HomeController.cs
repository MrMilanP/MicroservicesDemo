using MicroservicesDemo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using UserService.Models;

namespace MicroservicesDemo.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // Dodaj View za registraciju korisnika

        public IActionResult RegisterUser()
        {
            return View();
        }

        // Prikaz stranice za prijavu korisnika
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel loginModel)
        {
            // Kreiraj HttpClient za UserService pomoću IHttpClientFactory
            var client = _httpClientFactory.CreateClient("UserServiceClient");

            // Pozovi UserService API za prijavu
            var response = await client.PostAsJsonAsync("https://localhost:7033/api/auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
                return View();
            }

            // Parsiraj JWT token iz odgovora
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jwtToken = JsonSerializer.Deserialize<JsonElement>(jsonResponse).GetProperty("token").GetString();

            // Kreiraj kolačić za Cookie autentifikaciju
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginModel.Email),
            new Claim("JWT", jwtToken)  // Sačuvaj JWT u kolačiću (ako je potrebno za API pozive)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("UserProfile");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Dodaj View za prikaz korisničkog profila
        [Authorize]  // Ova akcija će biti zaštićena i traži prijavu
        public IActionResult UserProfile()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            ViewData["Title"] = "Add User";
            // Debugging log
            System.Diagnostics.Debug.WriteLine(ViewData["Title"]); // Proveri da li je null
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
