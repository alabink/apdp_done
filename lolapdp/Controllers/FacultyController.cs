using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly CSVService _csvService;

        public FacultyController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name;
            var courses = _csvService.GetFacultyCourses(username);
            return View(courses);
        }

        [HttpGet]
        public IActionResult AssignGrade(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest();
            }

            var course = _csvService.GetAllCourses()
                .FirstOrDefault(c => c.CourseId == courseId && 
                    c.Faculty.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase));

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            var students = _csvService.GetEnrolledStudents(courseId);
            
            // Lấy điểm của tất cả sinh viên trong khóa học
            var grades = new Dictionary<string, Grade>();
            foreach (var student in students)
            {
                var studentGrades = _csvService.GetStudentGrades(student.Username, courseId);
                if (studentGrades.Any())
                {
                    grades[student.Username] = studentGrades.First();
                }
            }
            ViewBag.StudentGrades = grades;

            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignGrade([FromBody] GradeUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.CourseId) || string.IsNullOrEmpty(model.Username))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            try
            {
                var course = _csvService.GetAllCourses()
                    .FirstOrDefault(c => c.CourseId == model.CourseId && 
                        c.Faculty.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase));

                if (course == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khóa học" });
                }

                var grade = new Grade
                {
                    Username = model.Username,
                    CourseId = model.CourseId,
                    Score = model.Score,
                    GradeDate = DateTime.Now
                };

                _csvService.AddGrade(grade);

                return Json(new
                {
                    success = true,
                    score = grade.Score.ToString("F1"),
                    gradeDate = grade.GradeDate.ToString("dd/MM/yyyy HH:mm"),
                    message = "Điểm đã được cập nhật thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class GradeUpdateModel
        {
            public string CourseId { get; set; }
            public string Username { get; set; }
            public decimal Score { get; set; }
        }

        public class GradeDistribution
        {
            public int Excellent { get; set; } // >= 90
            public int Good { get; set; }      // >= 70
            public int AverageGrade { get; set; }   // >= 50
            public int Poor { get; set; }      // < 50
        }

        public IActionResult GradeStatistics()
        {
            var courses = _csvService.GetFacultyCourses(User.Identity.Name);
            var allGradeStats = new List<(Course Course, GradeDistribution Stats)>();

            foreach (var course in courses)
            {
                var students = _csvService.GetEnrolledStudents(course.CourseId);
                var gradeStats = new GradeDistribution();

                foreach (var student in students)
                {
                    var studentGrades = _csvService.GetStudentGrades(student.Username, course.CourseId);
                    if (studentGrades.Any())
                    {
                        var grade = studentGrades.First().Score;
                        if (grade >= 90) gradeStats.Excellent++;
                        else if (grade >= 70) gradeStats.Good++;
                        else if (grade >= 50) gradeStats.AverageGrade++;
                        else gradeStats.Poor++;
                    }
                }

                allGradeStats.Add((course, gradeStats));
            }

            return View(allGradeStats);
        }
    }
}
//dbl