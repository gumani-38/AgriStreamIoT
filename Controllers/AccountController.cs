using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgriStreamIoT.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In production, validate credentials against Azure SQL Database (Username, Password, Role)
                // For demonstration, verifying a default admin account:
                if (model.Username == "admin" && model.Password == "password123")
                {
                    // Redirect to the main Telemetry Dashboard on successful authentication
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // Handle sign-out logic here if using ASP.NET Core Identity/Cookie auth
            return RedirectToAction("Login");
        }
    }
}