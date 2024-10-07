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
using Microsoft.OpenApi.Models;
using UserMicroservice.Models;
using Microsoft.OpenApi.Any;


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

// Registruje Swagger generator u servisni kontejner, omogućava generisanje Swagger dokumentacije za API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserMicroservice API", Version = "v1" });

    // Konfiguriši `JWT` autorizaciju
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Unesi 'Bearer' [space] token u polje ispod.\nPrimer: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Globalna konfiguracija sigurnosnih zahteva
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

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

// Aktivira Swagger middleware koji generiše Swagger JSON dokumentaciju na /swagger/v1/swagger.json
app.UseSwagger();

// Aktivira Swagger UI (korisnički interfejs) na /swagger URL-u, gde možeš vizuelno pregledati i testirati API rute
app.UseSwaggerUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
