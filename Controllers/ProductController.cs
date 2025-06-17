using CompanyDirectory.Data;
using CompanyDirectory.Models;
using CompanyDirectory.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Reusable breadcrumb helper
    private List<(string Label, string? Url)> BuildBreadcrumbs(Product product, string? last = null)
    {
        return new List<(string, string?)>
        {
            ("Dashboard", Url.Action("Dashboard", "Admin")),
            ("Companies", Url.Action("ManageCompanies", "Admin")),
            (product.Company?.Name ?? "Company", Url.Action("Index", "Product", new { companyId = product.CompanyId })),
            (last ?? product.Name, null)
        };
    }

    // List products
    [HttpGet]
    public async Task<IActionResult> Index(long companyId, string? search)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null) return NotFound();

        var productsQuery = _context.Products.Where(p => p.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(search) && search.Length >= 3)
        {
            search = search.ToLower();
            productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(search));
        }

        var products = await productsQuery.ToListAsync();

        ViewBag.CompanyId = companyId;
        ViewBag.CompanyName = company.Name;
        ViewBag.Search = search;

        return View(products);
    }

    // GET: Create Product
    public async Task<IActionResult> Create(long companyId)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null) return NotFound();

        ViewBag.Breadcrumbs = new List<(string, string?)>
        {
            ("Dashboard", Url.Action("Dashboard", "Admin")),
            ("Companies", Url.Action("ManageCompanies", "Admin")),
            (company.Name, Url.Action("Index", "Product", new { companyId })),
            ("Create Product", null)
        };

        var model = new ProductViewModel { CompanyId = companyId };
        return View(model);
    }

    // POST: Create Product
    [HttpPost]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            CompanyId = model.CompanyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (model.Images != null && model.Images.Count > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", model.CompanyId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var imagePaths = new List<string>();

            foreach (var image in model.Images)
            {
                var webpName = Guid.NewGuid().ToString() + ".webp";
                var filePath = Path.Combine(uploadsFolder, webpName);

                using var stream = image.OpenReadStream();
                using var img = Image.Load(stream);
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(800, 800)
                }));
                await img.SaveAsync(filePath, new WebpEncoder());

                imagePaths.Add($"/uploads/{model.CompanyId}/{webpName}");
            }

            product.ImagePaths = string.Join(",", imagePaths);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { companyId = model.CompanyId });
    }

    // GET: Edit Product
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        ViewBag.Breadcrumbs = BuildBreadcrumbs(product, "Edit Product");

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CompanyId = product.CompanyId
        };

        ViewBag.ExistingImages = product.ImagePaths?.Split(',') ?? new string[0];
        return View(model);
    }

    // POST: Edit Product
    [HttpPost]
    public async Task<IActionResult> Edit(long id, ProductViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = model.Name;
        product.Description = model.Description;
        product.UpdatedAt = DateTime.UtcNow;

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", model.CompanyId.ToString());
        Directory.CreateDirectory(uploadsFolder);

        if (model.Images != null && model.Images.Count > 0)
        {
            var imagePaths = new List<string>();

            foreach (var image in model.Images)
            {
                var webpName = Guid.NewGuid().ToString() + ".webp";
                var filePath = Path.Combine(uploadsFolder, webpName);

                using var stream = image.OpenReadStream();
                using var img = Image.Load(stream);
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(800, 800)
                }));
                await img.SaveAsync(filePath, new WebpEncoder());

                imagePaths.Add($"/uploads/{model.CompanyId}/{webpName}");
            }

            product.ImagePaths = string.Join(",", imagePaths);
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { companyId = model.CompanyId });
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        ViewBag.CompanyName = product.Company?.Name;
        ViewBag.CompanyId = product.CompanyId;
        ViewBag.ProductName = product.Name;

        return View(product);
    }

    // GET: Confirm Delete
    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        ViewBag.Breadcrumbs = BuildBreadcrumbs(product, "Delete Product");

        return View(product);
    }

    // POST: Confirm Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        if (!string.IsNullOrEmpty(product.ImagePaths))
        {
            foreach (var path in product.ImagePaths.Split(','))
            {
                var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
        }

        long companyId = product.CompanyId;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { companyId });
    }
}
