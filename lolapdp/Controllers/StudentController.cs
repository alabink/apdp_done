using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly CSVService _csvService;

        public StudentController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name;
            var courses = _csvService.GetStudentCourses(username);
            return View(courses);
        }

        public IActionResult Grades(string courseId = null)
        {
            var username = User.Identity.Name;
            var grades = string.IsNullOrEmpty(courseId) 
                ? _csvService.GetStudentGrades(username)
                : _csvService.GetStudentGrades(username, courseId);
            
            // Get course information for each grade
            var courses = _csvService.GetAllCourses()
                .ToDictionary(c => c.CourseId);

            ViewBag.Courses = courses;
            ViewBag.SelectedCourseId = courseId;
            
            return View(grades);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var username = User.Identity.Name;
            var enrolledCourses = _csvService.GetStudentCourses(username)
                .Select(c => c.CourseId)
                .ToList();

            var availableCourses = _csvService.GetAllCourses()
                .Where(c => c.IsActive && !enrolledCourses.Contains(c.CourseId))
                .ToList();

            return View(availableCourses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest();
            }

            try
            {
                _csvService.AddStudentToCourse(User.Identity.Name, courseId);
                TempData["Message"] = "Đăng ký khóa học thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Register));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest();
            }

            try
            {
                _csvService.RemoveStudentFromCourse(User.Identity.Name, courseId);
                TempData["Message"] = "Hủy đăng ký khóa học thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
//dbl