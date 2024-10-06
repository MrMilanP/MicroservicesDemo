using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using UserMicroservice.Data;
using UserMicroservice.Repositories;
using UserMicroservice.Services;
using MicroservicesShared.Configuration;


var builder = WebApplication.CreateBuilder(args);


// 1. Kreiraj `JwtSettings` iz konfiguracije
var jwtSettings = new MicroservicesShared.Configuration.JwtSettings
{
    Issuer = builder.Configuration["Jwt:Issuer"],
    Audience = builder.Configuration["Jwt:Audience"],
    Key = builder.Configuration["Jwt:Key"]
};

// 2. Proveri da li su sve vrednosti pravilno postavljene
if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
    string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
    string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new ArgumentNullException("JWT konfiguracija nije validna. Proveri appsettings.json.");
}

// 3. Registruj `JwtSettings` kao Singleton
builder.Services.AddSingleton(jwtSettings);

// 4. Kreiraj `SymmetricSecurityKey` i `SigningCredentials` jednom i registruj kao Singleton
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
builder.Services.AddSingleton(signingCredentials);

// 5. Postavi JWT konfiguraciju koristeći `JwtSettings` i `SigningCredentials`
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = signingKey
        };
    });

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


// Registruj interfejse i implementacije
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserMicroservice.Services.UserService>();



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
