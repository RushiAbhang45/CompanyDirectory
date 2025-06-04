using Bogus;
using CompanyDirectory.Data.CompanyDirectory.Data;
using CompanyDirectory.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class DbSeeder
{
    public static void SeedCompanies(AppDbContext context)
    {
        // Apply any pending migrations
        context.Database.Migrate();

        // ✅ Only seed if table is empty (prevents duplicate seeding)
        if (context.Companies.Any())
            return;

        var faker = new Faker<Company>()
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.Description, f => f.Company.CatchPhrase())
            .RuleFor(c => c.LogoUrl, _ => "https://via.placeholder.com/150")
            .RuleFor(c => c.Website, f => f.Internet.Url())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
            .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());

        var companies = faker.Generate(1000);
        context.Companies.AddRange(companies);
        context.SaveChanges();

        Console.WriteLine("✅ Seeded 1000 companies.");
    }
}
