using CompanyDirectory.Data;
using Microsoft.EntityFrameworkCore;
using CompanyDirectory.Models;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enable Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication service (required for UseAuthentication)
builder.Services.AddAuthentication("CookieAuth") // Use a simple cookie scheme
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/Login";
    });

// Build the app
var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.SeedCompanies(db); // Seeds Admin and Companies
}

// Middleware pipeline
app.UseStaticFiles();

app.UseRouting();

// Enable session before auth middleware
app.UseSession();

// Authentication & Authorization middleware order matters
app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Register}/{id?}");

app.Run();
