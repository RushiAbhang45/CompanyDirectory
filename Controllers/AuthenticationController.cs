﻿using CompanyDirectory.Data;
using CompanyDirectory.Models;
using CompanyDirectory.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using BCrypt.Net; // Add this using

namespace CompanyDirectory.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AppDbContext _context;
        public AuthenticationController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Register()
        {
            var random = new Random();
            var model = new RegisterViewModel
            {
                CaptchaNum1 = random.Next(1, 10),
                CaptchaNum2 = random.Next(1, 10)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Validate captcha answer
            if (string.IsNullOrEmpty(model.CaptchaAnswer) ||
                !int.TryParse(model.CaptchaAnswer, out int userAnswer) ||
                userAnswer != model.CaptchaNum1 + model.CaptchaNum2)
            {
                ModelState.AddModelError("CaptchaAnswer", "Incorrect captcha answer.");
            }

            if (!ModelState.IsValid) return View(model);

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Mobile = model.Mobile,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role  // ✅ Get role from dropdown selection
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }

            // Store user data in session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.FirstName ?? "");
            HttpContext.Session.SetString("UserRole", user.Role ?? "User");

            // Redirect based on role
            if ((user.Role ?? "User") == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Company");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Email not found.";
                return View();
            }

            // Mock reset logic
            TempData["ResetLink"] = Url.Action("ResetPassword", "Authentication", new { email = user.Email }, Request.Scheme);
            return RedirectToAction("ShowResetLink");
        }

        public IActionResult ShowResetLink() => View();

        [HttpGet]
        public IActionResult ResetPassword(string email) => View(new ResetPasswordViewModel { Email = email });

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}
