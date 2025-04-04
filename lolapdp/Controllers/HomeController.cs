using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lolapdp.Controllers
{
    public class HomeController : Controller
    {
        private readonly CSVService _csvService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(CSVService csvService, ILogger<HomeController> logger)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }

            var courses = _csvService.GetAllCourses()
                .Where(c => c.IsActive)
                .ToList();
            return View(courses);
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var username = User.Identity.Name;

            if (User.IsInRole("Admin"))
            {
                var users = _csvService.GetAllUsers();
                var courses = _csvService.GetAllCourses();

                ViewBag.TotalUsers = users.Count;
                ViewBag.TotalStudents = users.Count(u => u.Role == "Student");
                ViewBag.TotalFaculty = users.Count(u => u.Role == "Faculty");
                ViewBag.TotalCourses = courses.Count;
            }
            else if (User.IsInRole("Faculty"))
            {
                var teachingCourses = _csvService.GetFacultyCourses(username);
                var enrolledStudents = new HashSet<string>();

                foreach (var course in teachingCourses)
                {
                    var students = _csvService.GetEnrolledStudents(course.CourseId);
                    foreach (var student in students)
                    {
                        enrolledStudents.Add(student.Username);
                    }
                }

                ViewBag.TeachingCourses = teachingCourses.Count;
                ViewBag.TotalStudents = enrolledStudents.Count;
                ViewBag.RecentCourses = teachingCourses.Take(5).ToList();
            }
            else if (User.IsInRole("Student"))
            {
                var enrolledCourses = _csvService.GetStudentCourses(username);
                var grades = _csvService.GetStudentGrades(username);
                var availableCourses = _csvService.GetAllCourses()
                    .Where(c => c.IsActive && !enrolledCourses.Any(ec => ec.CourseId == c.CourseId))
                    .ToList();

                ViewBag.EnrolledCourses = enrolledCourses.Count;
                ViewBag.AvailableCourses = availableCourses.Count;
                ViewBag.AverageGrade = grades.Any() ? grades.Average(g => g.Score).ToString("F1") : "N/A";
                ViewBag.RecentCourses = enrolledCourses.Take(5).ToList();

                // Tạo thông báo mẫu
                ViewBag.Notifications = new List<object>
                {
                    new { Date = DateTime.Now.AddDays(-1), Message = "Bạn có bài tập mới trong khóa học " + enrolledCourses.FirstOrDefault()?.CourseName },
                    new { Date = DateTime.Now.AddDays(-3), Message = "Điểm của bạn đã được cập nhật" }
                };
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
//dbl