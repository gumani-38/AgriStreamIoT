using AgriStreamIoT.Data; // Ensure this matches your DbContext namespace
using AgriStreamIoT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriStreamIoT.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Inject ApplicationDbContext alongside IConfiguration
        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var input = model.Username?.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == input || u.Email == input);

            if (user == null)
            {
                ViewBag.DebugMessage = "USER NOT FOUND IN DATABASE";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ViewBag.DebugMessage = "PASSWORD HASH IS EMPTY";
                return View(model);
            }

            bool passwordValid;

            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.PasswordHash.Trim()
                );
            }
            catch (Exception ex)
            {
                ViewBag.DebugMessage = "BCrypt ERROR: " + ex.Message;
                return View(model);
            }

            if (!passwordValid)
            {
                ViewBag.DebugMessage = "USER FOUND, BUT PASSWORD DOES NOT MATCH";
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Role, user.Role ?? "User")
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Sign out the user cookie session
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}