using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lolapdp.Controllers
{
    public class AccountController : Controller
    {
        private readonly CSVService _csvService;

        public AccountController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Vui lòng nhập tên đăng nhập và mật khẩu";
                return View();
            }

            var user = _csvService.AuthenticateUser(username, password);

            if (user == null)
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim("Email", user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["Message"] = $"Chào mừng {user.FullName}";

            // Chuyển hướng đến trang Dashboard
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin";
                return View(user);
            }

            try
            {
                user.Role = "Student"; // Mặc định đăng ký là sinh viên
                _csvService.AddUser(user);
                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(user);
            }
        }
    }
}


//dbl