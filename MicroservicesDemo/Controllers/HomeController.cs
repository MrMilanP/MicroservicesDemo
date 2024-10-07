using MicroservicesDemo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using UserMicroservice.Models;

namespace MicroservicesDemo.Controllers
{
    // Ovim atributom sakrij ceo kontroler iz Swagger dokumentacije
    [ApiExplorerSettings(IgnoreApi = true)]
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
            // 1. Kreiraj URL sa query string parametrima
            var url = $"https://localhost:7033/api/auth/login?email={loginModel.Email}&password={loginModel.Password}";

            // 2. Kreiraj HttpClient za UserMicroservice pomoću IHttpClientFactory
            var client = _httpClientFactory.CreateClient("UserServiceClient");

            // 3. Pošalji POST zahtev sa praznim telom
            var response = await client.PostAsync(url, null);

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
                new Claim("jwt", jwtToken)  // Sačuvaj JWT u kolačiću (ako je potrebno za API pozive)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Postavi JWT token u ViewBag kako bi bio dostupan u View-u
            // ViewBag.JwtToken = jwtToken;  // Ova linija je isključena zato što `ViewBag` ne može preživeti `RedirectToAction`.

            // Umesto `ViewBag`, koristimo `TempData` da sačuvamo `JWT` token.
            // `TempData` može da preživi `Redirect` pozive, što znači da će `JwtToken` biti dostupan i nakon `RedirectToAction`.
            // Ovo je važno jer želimo da token bude prosleđen na `UserProfile` View nakon prijave.
            TempData["JwtToken"] = jwtToken;

            // `RedirectToAction` se koristi umesto `return View`, jer `Redirect` kreira novi HTTP zahtev.
            // To osigurava da se `User.Identity` pravilno osveži, što omogućava `UserProfile` View-u da prepozna korisnika kao prijavljenog.
            return RedirectToAction("UserProfile");
        }
        public IActionResult Logout()
        {
            // Odjavljuje korisnika sa trenutne `CookieAuthentication` šeme.
            // Briše `Cookie` koji ASP.NET Core koristi za autentifikaciju.
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Ranije: Vraća korisnika na `Login` akciju, što bi kreiralo novi HTTP zahtev.
            // return RedirectToAction("Login");

            // Sada: Prikazuje `Logout` View koji može da sadrži JavaScript za brisanje `localStorage`.
            // Ova izmena je dodata kako bi omogućila klijentskoj strani (JavaScript) da obriše `JWT` token 
            // koji se nalazi u `localStorage`, jer `SignOutAsync` briše samo `Cookie` i ne može obrisati `localStorage`.
            return View("Logout");
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
