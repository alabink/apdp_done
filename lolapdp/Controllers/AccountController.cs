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
    public class AccountController : Controller // Controller xử lý các chức năng liên quan đến tài khoản
    {
        private readonly CSVService _csvService; // Dịch vụ CSV để thao tác với dữ liệu

        public AccountController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService)); // Khởi tạo dịch vụ CSV
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home"); // Chuyển hướng đến Dashboard nếu đã đăng nhập
            }
            return View(); // Trả về view đăng nhập
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Vui lòng nhập tên đăng nhập và mật khẩu"; // Thông báo lỗi nếu thiếu thông tin
                return View();
            }

            var user = _csvService.AuthenticateUser(username, password); // Xác thực người dùng

            if (user == null)
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng"; // Thông báo lỗi nếu thông tin không đúng
                return View();
            }

            // Tạo danh sách claims cho người dùng
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), // Username
                new Claim(ClaimTypes.Role, user.Role), // Vai trò
                new Claim("FullName", user.FullName), // Tên đầy đủ
                new Claim("Email", user.Email) // Email
            };

            // Tạo identity với claims
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Lưu cookie đăng nhập
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24) // Cookie hết hạn sau 24 giờ
            };

            // Đăng nhập người dùng
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["Message"] = $"Chào mừng {user.FullName}"; // Thông báo chào mừng

            // Chuyển hướng đến trang Dashboard
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Đăng xuất người dùng
            return RedirectToAction("Login"); // Chuyển hướng về trang đăng nhập
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Trả về view đăng ký
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin"; // Thông báo lỗi nếu thiếu thông tin
                return View(user);
            }

            try
            {
                user.Role = "Student"; // Mặc định đăng ký là sinh viên
                _csvService.AddUser(user); // Thêm người dùng mới
                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập"; // Thông báo thành công
                return RedirectToAction("Login"); // Chuyển hướng về trang đăng nhập
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi nếu có ngoại lệ
                return View(user);
            }
        }
    }
}


//dbl