using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Student")] // Chỉ cho phép người dùng có vai trò Student truy cập
    public class StudentController : Controller
    {
        private readonly CSVService _csvService; // Dịch vụ CSV để thao tác với dữ liệu

        public StudentController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService)); // Khởi tạo dịch vụ CSV
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name; // Lấy username của sinh viên đang đăng nhập
            var courses = _csvService.GetStudentCourses(username); // Lấy danh sách khóa học của sinh viên
            return View(courses); // Trả về view với danh sách khóa học
        }

        public IActionResult Grades(string courseId = null)
        {
            var username = User.Identity.Name; // Lấy username của sinh viên đang đăng nhập
            var grades = string.IsNullOrEmpty(courseId) 
                ? _csvService.GetStudentGrades(username) // Lấy tất cả điểm của sinh viên
                : _csvService.GetStudentGrades(username, courseId); // Lấy điểm của sinh viên theo khóa học
            
            // Lấy thông tin khóa học cho mỗi điểm
            var courses = _csvService.GetAllCourses()
                .ToDictionary(c => c.CourseId); // Chuyển danh sách khóa học thành dictionary để dễ tra cứu

            ViewBag.Courses = courses; // Lưu thông tin khóa học vào ViewBag
            ViewBag.SelectedCourseId = courseId; // Lưu mã khóa học được chọn vào ViewBag
            
            return View(grades); // Trả về view với danh sách điểm
        }

        [HttpGet]
        public IActionResult Register()
        {
            var username = User.Identity.Name; // Lấy username của sinh viên đang đăng nhập
            var enrolledCourses = _csvService.GetStudentCourses(username)
                .Select(c => c.CourseId)
                .ToList(); // Lấy danh sách mã khóa học đã đăng ký

            var availableCourses = _csvService.GetAllCourses()
                .Where(c => c.IsActive && !enrolledCourses.Contains(c.CourseId))
                .ToList(); // Lấy danh sách khóa học có thể đăng ký (đang hoạt động và chưa đăng ký)

            return View(availableCourses); // Trả về view với danh sách khóa học có thể đăng ký
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest(); // Trả về lỗi nếu không có mã khóa học
            }

            try
            {
                _csvService.AddStudentToCourse(User.Identity.Name, courseId); // Thêm sinh viên vào khóa học
                TempData["Message"] = "Đăng ký khóa học thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }

            return RedirectToAction(nameof(Register)); // Chuyển hướng về trang đăng ký
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest(); // Trả về lỗi nếu không có mã khóa học
            }

            try
            {
                _csvService.RemoveStudentFromCourse(User.Identity.Name, courseId); // Xóa sinh viên khỏi khóa học
                TempData["Message"] = "Hủy đăng ký khóa học thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }

            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang chính
        }
    }
}
//dbl