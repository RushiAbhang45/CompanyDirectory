using CompanyDirectory.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyDirectory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
