using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Konfiguriši autentifikaciju samo jednom
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";  // Ako nije prijavljen, preusmeri na Login
        options.AccessDeniedPath = "/Home/AccessDenied";  // Ako nema prava pristupa
    });

// Konfiguriši konekcioni string
var str = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(str));

// Dodaj autorizaciju
builder.Services.AddAuthorization();

// Dodaj servise za kontrolere sa view-ovima
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();  // Aktiviraj autentifikaciju
app.UseAuthorization();   // Aktiviraj autorizaciju

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
