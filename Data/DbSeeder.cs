using Bogus;
using CompanyDirectory.Data;
using CompanyDirectory.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
public static class DbSeeder
{
    public static void SeedCompanies(AppDbContext context)
    {
        // Apply migrations
        context.Database.Migrate();

        using var transaction = context.Database.BeginTransaction();

        try
        {
            // Seed Admin User if not exists
            if (!context.Users.Any(u => u.Email == "admin@mid.com"))
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");

                var admin = new ApplicationUser
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@mid.com",
                    Mobile = "9999999999",
                    PasswordHash = passwordHash,
                    Role = "Admin"
                };

                context.Users.Add(admin);
                context.SaveChanges();  // Save admin immediately
                Console.WriteLine("✅ Admin user seeded: admin@mid.com / admin123");
            }

            // Seed Companies if none exist
            if (!context.Companies.Any())
            {
                var companyFaker = new Faker<Company>()
                    .RuleFor(c => c.Name, f => f.Company.CompanyName())
                    .RuleFor(c => c.Description, f => f.Company.CatchPhrase())
                    .RuleFor(c => c.LogoUrl, _ => "https://via.placeholder.com/150")
                    .RuleFor(c => c.Website, f => f.Internet.Url())
                    .RuleFor(c => c.CreatedAt, f => f.Date.Past(2))
                    .RuleFor(c => c.UpdatedAt, f => f.Date.Recent(30));

                var companies = companyFaker.Generate(1000);
                context.Companies.AddRange(companies);
                context.SaveChanges();
                Console.WriteLine("✅ Seeded 1000 companies.");
            }

            // Seed Products for companies missing products
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription());

            var companiesWithoutProducts = context.Companies
                .Where(c => !context.Products.Any(p => p.CompanyId == c.Id))
                .ToList();

            var allNewProducts = new List<Product>();
            var random = new Random();

            foreach (var company in companiesWithoutProducts)
            {
                int numberOfProducts = random.Next(5, 11);
                var products = productFaker.Clone()
                    .RuleFor(p => p.CompanyId, _ => company.Id)
                    .Generate(numberOfProducts);

                allNewProducts.AddRange(products);
            }

            if (allNewProducts.Any())
            {
                context.Products.AddRange(allNewProducts);
                context.SaveChanges();
                Console.WriteLine($"✅ Seeded {allNewProducts.Count} products for {companiesWithoutProducts.Count} companies.");
            }
            else
            {
                Console.WriteLine("ℹ️ All companies already have products. No new products were seeded.");
            }

            // Commit transaction after all saves
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine("❌ Error seeding database: " + ex.Message);
            throw;
        }
    }
}
