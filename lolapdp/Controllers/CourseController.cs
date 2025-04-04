using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly CSVService _csvService;

        public CourseController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
        }

        public IActionResult Index()
        {
            var courses = _csvService.GetAllCourses();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.FacultyList = facultyList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (course == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    course.IsActive = true;
                    _csvService.AddCourse(course);
                    TempData["Message"] = "Khóa học đã được tạo thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.FacultyList = facultyList;
            return View(course);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var course = _csvService.GetAllCourses()
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.FacultyList = facultyList;

            // Get enrolled students
            var enrolledStudents = _csvService.GetEnrolledStudents(id);
            ViewBag.EnrolledStudents = enrolledStudents;

            // Get student grades
            var grades = _csvService.GetStudentGrades(id);
            ViewBag.StudentGrades = grades.ToDictionary(g => g.Username);

            // Get available students (not enrolled)
            var allStudents = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.AvailableStudents = allStudents
                .Where(s => !enrolledStudents.Any(es => es.Username == s.Username))
                .ToList();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Course course)
        {
            if (string.IsNullOrEmpty(id) || course == null || id != course.CourseId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _csvService.UpdateCourse(course);
                    TempData["Message"] = "Khóa học đã được cập nhật thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.FacultyList = facultyList;
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateGrade(string courseId, string username, decimal score)
        {
            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }

            try
            {
                var grade = new Grade
                {
                    CourseId = courseId,
                    Username = username,
                    Score = score,
                    GradeDate = DateTime.Now
                };

                _csvService.AddGrade(grade);
                TempData["Message"] = "Điểm số đã được cập nhật thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var course = _csvService.GetAllCourses()
                    .FirstOrDefault(c => c.CourseId == id);

                if (course != null)
                {
                    course.IsActive = false;
                    _csvService.UpdateCourse(course);
                    TempData["Message"] = "Khóa học đã được xóa thành công";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnrollStudent(string courseId, string username)
        {
            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }

            try
            {
                _csvService.AddStudentToCourse(username, courseId);
                TempData["Message"] = "Sinh viên đã được đăng ký thành công vào khóa học";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveStudent(string courseId, string username)
        {
            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }

            try
            {
                _csvService.RemoveStudentFromCourse(username, courseId);
                TempData["Message"] = "Sinh viên đã được xóa khỏi khóa học";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = courseId });
        }
    }
}