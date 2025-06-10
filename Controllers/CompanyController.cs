using CompanyDirectory.Controllers;
using CompanyDirectory.Data;
using CompanyDirectory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CompanyController : Controller
{
    private readonly AppDbContext _context;
    private const int PageSize = 10;

    public CompanyController(AppDbContext context)
    {
        _context = context;
    }

    // Helper: Check if the current user is an Admin
    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "Admin";
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
    public async Task<IActionResult> Details(long id)
    {
        var company = await _context.Companies
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        return View(company);
    }

    // GET: Company/Create
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
        return View();
    }

    // POST: Company/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");

        if (ModelState.IsValid)
        {
            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(company);
    }

    // GET: Company/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
        if (id == null) return NotFound();

        var company = await _context.Companies.FindAsync(id);
        if (company == null) return NotFound();

        return View(company);
    }

    // POST: Company/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Company company)
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
        if (id != company.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(company);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(company.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(company);
    }

    // GET: Company/Delete/5
    public async Task<IActionResult> Delete(long? id)
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");
        if (id == null) return NotFound();

        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null) return NotFound();

        return View(company);
    }

    // POST: Company/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        if (!IsAdmin()) return RedirectToAction(nameof(AuthenticationController.Login), "Authentication");

        var company = await _context.Companies.FindAsync(id);
        if (company != null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CompanyExists(long id)
    {
        return _context.Companies.Any(e => e.Id == id);
    }
}
