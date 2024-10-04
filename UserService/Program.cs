using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);
//Debug.WriteLine("Send to debug output. {} " + builder.Configuration.GetConnectionString("DefaultConnection"));
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Homeuser}/{action=Index}/{id?}");

app.Run();
