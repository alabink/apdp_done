using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lolapdp.Controllers
{
    public class HomeController : Controller // Controller chính xử lý các trang chung
    {
        private readonly CSVService _csvService; // Dịch vụ CSV để thao tác với dữ liệu
        private readonly ILogger<HomeController> _logger; // Logger để ghi log

        public HomeController(CSVService csvService, ILogger<HomeController> logger)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService)); // Khởi tạo dịch vụ CSV
            _logger = logger; // Khởi tạo logger
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard"); // Chuyển hướng đến Dashboard nếu đã đăng nhập
            }

            var courses = _csvService.GetAllCourses()
                .Where(c => c.IsActive)
                .ToList(); // Lấy danh sách khóa học đang hoạt động
            return View(courses); // Trả về view với danh sách khóa học
        }

        [Authorize] // Yêu cầu đăng nhập để truy cập
        public IActionResult Dashboard()
        {
            var username = User.Identity.Name; // Lấy username của người dùng đang đăng nhập

            if (User.IsInRole("Admin")) // Nếu là Admin
            {
                var users = _csvService.GetAllUsers(); // Lấy tất cả người dùng
                var courses = _csvService.GetAllCourses(); // Lấy tất cả khóa học

                ViewBag.TotalUsers = users.Count; // Tổng số người dùng
                ViewBag.TotalStudents = users.Count(u => u.Role == "Student"); // Tổng số sinh viên
                ViewBag.TotalFaculty = users.Count(u => u.Role == "Faculty"); // Tổng số giảng viên
                ViewBag.TotalCourses = courses.Count; // Tổng số khóa học
            }
            else if (User.IsInRole("Faculty")) // Nếu là Giảng viên
            {
                var teachingCourses = _csvService.GetFacultyCourses(username); // Lấy danh sách khóa học đang dạy
                var enrolledStudents = new HashSet<string>(); // Danh sách sinh viên đã ghi danh (không trùng lặp)

                foreach (var course in teachingCourses)
                {
                    var students = _csvService.GetEnrolledStudents(course.CourseId); // Lấy danh sách sinh viên trong khóa học
                    foreach (var student in students)
                    {
                        enrolledStudents.Add(student.Username); // Thêm sinh viên vào danh sách
                    }
                }

                ViewBag.TeachingCourses = teachingCourses.Count; // Số khóa học đang dạy
                ViewBag.TotalStudents = enrolledStudents.Count; // Tổng số sinh viên
                ViewBag.RecentCourses = teachingCourses.Take(5).ToList(); // 5 khóa học gần đây
            }
            else if (User.IsInRole("Student")) // Nếu là Sinh viên
            {
                var enrolledCourses = _csvService.GetStudentCourses(username); // Lấy danh sách khóa học đã đăng ký
                var grades = _csvService.GetStudentGrades(username); // Lấy điểm của sinh viên
                var availableCourses = _csvService.GetAllCourses()
                    .Where(c => c.IsActive && !enrolledCourses.Any(ec => ec.CourseId == c.CourseId))
                    .ToList(); // Lấy danh sách khóa học có thể đăng ký

                ViewBag.EnrolledCourses = enrolledCourses.Count; // Số khóa học đã đăng ký
                ViewBag.AvailableCourses = availableCourses.Count; // Số khóa học có thể đăng ký
                ViewBag.AverageGrade = grades.Any() ? grades.Average(g => g.Score).ToString("F1") : "N/A"; // Điểm trung bình
                ViewBag.RecentCourses = enrolledCourses.Take(5).ToList(); // 5 khóa học gần đây

                // Tạo thông báo mẫu cho sinh viên
                ViewBag.Notifications = new List<object>
                {
                    new { Date = DateTime.Now.AddDays(-1), Message = "Bạn có bài tập mới trong khóa học " + enrolledCourses.FirstOrDefault()?.CourseName },
                    new { Date = DateTime.Now.AddDays(-3), Message = "Điểm của bạn đã được cập nhật" }
                };
            }

            return View(); // Trả về view dashboard tương ứng với vai trò
        }

        public IActionResult Privacy()
        {
            return View(); // Trả về trang chính sách bảo mật
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Không cache trang lỗi
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Trả về trang lỗi với ID yêu cầu
        }
    }
}
//dbl