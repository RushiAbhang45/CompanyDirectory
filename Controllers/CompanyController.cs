using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyDirectory.Data;
using CompanyDirectory.Models;

public class CompanyController : Controller
{
    private readonly AppDbContext _context;
    private const int PageSize = 10;

    public CompanyController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Company/Index?page=1
    public async Task<IActionResult> Index(int page = 1)
    {
        var totalCompanies = await _context.Companies.CountAsync();

        var totalPages = (int)Math.Ceiling(totalCompanies / (double)PageSize);

        var companies = await _context.Companies
            .OrderBy(c => c.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(companies);
    }

    // GET: Company/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var company = await _context.Companies
            .Include(c => c.Products)  // Include products
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        return View(company);
    }
}
