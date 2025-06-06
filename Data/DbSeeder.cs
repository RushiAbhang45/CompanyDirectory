using Bogus;
using CompanyDirectory.Data;
using CompanyDirectory.Models;
using Microsoft.EntityFrameworkCore;

public class DbSeeder
{
    public static void SeedCompanies(AppDbContext context)
    {
        context.Database.Migrate();

        if (!context.Companies.Any())
        {
            var companyFaker = new Faker<Company>()
                .RuleFor(c => c.Name, f => f.Company.CompanyName())
                .RuleFor(c => c.Description, f => f.Company.CatchPhrase())
                .RuleFor(c => c.LogoUrl, _ => "https://via.placeholder.com/150")
                .RuleFor(c => c.Website, f => f.Internet.Url())
                .RuleFor(c => c.CreatedAt, f => f.Date.Past())
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());

            var companies = companyFaker.Generate(1000);
            context.Companies.AddRange(companies);
            context.SaveChanges();

            Console.WriteLine("✅ Seeded 1000 companies.");
        }

        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription());

        // ✅ Use database query to check if company has products
        var companiesWithoutProducts = context.Companies
            .Where(c => !context.Products.Any(p => p.CompanyId == c.Id))
            .ToList();

        var allNewProducts = new List<Product>();
        var random = new Random();

        foreach (var company in companiesWithoutProducts)
        {
            int numberOfProducts = random.Next(5, 11); // 5 to 10 products
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
    }
}
