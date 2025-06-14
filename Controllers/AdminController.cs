using Microsoft.AspNetCore.Mvc;
using CompanyDirectory.Data;
using CompanyDirectory.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyDirectory.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Role check helper
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            return View();
        }

        // List all companies
        public async Task<IActionResult> ManageCompanies()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        // Create Company - GET
        public IActionResult CreateCompany()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            return View();
        }

        // Create Company - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(Company company)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            if (ModelState.IsValid)
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageCompanies));
            }
            return View(company);
        }

        // Edit Company - GET
        public async Task<IActionResult> EditCompany(long? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            if (id == null) return NotFound();

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            return View(company);
        }

        // Edit Company - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(long id, Company company)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

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
                    if (!_context.Companies.Any(e => e.Id == company.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ManageCompanies));
            }
            return View(company);
        }

        // Delete Company - GET
        public async Task<IActionResult> DeleteCompany(long? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            if (id == null) return NotFound();

            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null) return NotFound();

            return View(company);
        }

        // Delete Company - POST
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyConfirmed(long id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageCompanies));
        }
        //                     Search functionality
        public async Task<IActionResult> SearchCompanies(string term)
        {
            if (!IsAdmin())
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(term))
            {
                var allCompanies = await _context.Companies.ToListAsync();
                return PartialView("CompanyTable", allCompanies);
            }

            var filtered = await _context.Companies
                .Where(c => c.Name.Contains(term) || c.Description.Contains(term))
                .ToListAsync();

            return PartialView("CompanyTable", filtered);
        }

        // AdminController.cs ///////////////////////////////////////////User Crud ADD User Update/Delete
        [HttpGet]
        public async Task<IActionResult> ViewUsers()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentication");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // Create - GET
        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");
            return View();
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                ModelState.AddModelError("Email", "Email already exists");

            if (!ModelState.IsValid)
                return View(user);

            // Set default password and timestamps
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Default@123");
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewUsers");
        }

        // Edit - GET
        public async Task<IActionResult> EditUser(long? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(long id, ApplicationUser user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");
            if (id != user.Id) return NotFound();

            // Exclude password from model validation
            ModelState.Remove("PasswordHash");

            if (!ModelState.IsValid)
                return View(user);

            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.Mobile = user.Mobile;
            existing.Role = user.Role;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("ViewUsers");
        }

        // Delete - GET
        public async Task<IActionResult> DeleteUser(long? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");

            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // Delete - POST
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewUsers");
        }
    }
}
