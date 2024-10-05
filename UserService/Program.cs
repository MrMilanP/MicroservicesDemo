


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



var builder = WebApplication.CreateBuilder(args);

// Dodaj JWT autentifikaciju u UserMicroservice API
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Dodaj servis za DbContext i poveži sa SQL Serverom
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Dodaj servise za kontrolere
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homeuser}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "user/{action=Index}/{id?}",
    defaults: new { controller = "User" });

app.Run();
