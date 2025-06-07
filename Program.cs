using CompanyDirectory.Data;
using Microsoft.EntityFrameworkCore;
using CompanyDirectory.Models;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider(); // Optional if using TempData
builder.Services.AddHttpContextAccessor(); // Needed for accessing session in views

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Run seeder
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.SeedCompanies(db);
}

// Middleware
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Must come before UseAuthorization
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Register}/{id?}");

app.Run();
