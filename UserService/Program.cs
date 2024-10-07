


// Program.cs u UserMicroservice projektu služi prvenstveno za razvoj i izolovano testiranje ovog mikroservisa.
// Kada se pokreće cela aplikacija iz MicroservicesDemo, ovaj fajl nije aktivan, jer MicroservicesDemo upravlja startup procesom.
// Sve konfiguracije iz UserMicroservice treba registrovati u Program.cs glavnog projekta (MicroservicesDemo).


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


// Ova konfiguracija omogućava UserMicroservice da se pokrene kao samostalna aplikacija.
// Ukoliko se UserMicroservice startuje nezavisno od MicroservicesDemo, koristiće sopstvene JWT postavke.
// U suprotnom, MicroservicesDemo projekt preuzima odgovornost za postavke autentifikacije.
// Kada se koristi kao nezavisan, JWT konfiguracija je bazirana na vrednostima iz MicroservicesShared projekta.


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

// Dodaj servis za DbContext i poveži sa SQL Serverom
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Dodaj servise za kontrolere
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registruje Swagger generator u servisni kontejner, omogućava generisanje Swagger dokumentacije za API
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Konfiguriši middleware za aplikaciju
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthentication();  // Aktiviraj autentifikaciju
app.UseAuthorization();   // Aktiviraj autorizaciju

// Aktivira Swagger middleware koji generiše Swagger JSON dokumentaciju na /swagger/v1/swagger.json
app.UseSwagger();

// Aktivira Swagger UI (korisnički interfejs) na /swagger URL-u, gde možeš vizuelno pregledati i testirati API rute
app.UseSwaggerUI();

app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homeuser}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "user/{action=Index}/{id?}",
    defaults: new { controller = "User" });

app.Run();
