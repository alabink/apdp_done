using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Faculty")] // Chỉ cho phép người dùng có vai trò Faculty truy cập
    public class FacultyController : Controller
    {
        private readonly CSVService _csvService; // Dịch vụ CSV để thao tác với dữ liệu

        public FacultyController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService)); // Khởi tạo dịch vụ CSV
        }

        public IActionResult Index()
        {
            var username = User.Identity.Name; // Lấy username của giảng viên đang đăng nhập
            var courses = _csvService.GetFacultyCourses(username); // Lấy danh sách khóa học của giảng viên
            return View(courses); // Trả về view với danh sách khóa học
        }

        [HttpGet]
        public IActionResult AssignGrade(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                return BadRequest(); // Trả về lỗi nếu không có mã khóa học
            }

            var course = _csvService.GetAllCourses()
                .FirstOrDefault(c => c.CourseId == courseId && 
                    c.Faculty.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)); // Tìm khóa học của giảng viên

            if (course == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy khóa học
            }

            ViewBag.Course = course; // Lưu thông tin khóa học vào ViewBag
            var students = _csvService.GetEnrolledStudents(courseId); // Lấy danh sách sinh viên đã ghi danh
            
            // Lấy điểm của tất cả sinh viên trong khóa học
            var grades = new Dictionary<string, Grade>();
            foreach (var student in students)
            {
                var studentGrades = _csvService.GetStudentGrades(student.Username, courseId); // Lấy điểm của sinh viên
                if (studentGrades.Any())
                {
                    grades[student.Username] = studentGrades.First(); // Lưu điểm vào dictionary
                }
            }
            ViewBag.StudentGrades = grades; // Lưu điểm của sinh viên vào ViewBag

            return View(students); // Trả về view với danh sách sinh viên
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignGrade([FromBody] GradeUpdateModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.CourseId) || string.IsNullOrEmpty(model.Username))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" }); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            try
            {
                var course = _csvService.GetAllCourses()
                    .FirstOrDefault(c => c.CourseId == model.CourseId && 
                        c.Faculty.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)); // Tìm khóa học của giảng viên

                if (course == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khóa học" }); // Trả về lỗi nếu không tìm thấy khóa học
                }

                var grade = new Grade
                {
                    Username = model.Username, // Username của sinh viên
                    CourseId = model.CourseId, // Mã khóa học
                    Score = model.Score, // Điểm số
                    GradeDate = DateTime.Now // Ngày chấm điểm
                };

                _csvService.AddGrade(grade); // Thêm điểm mới

                return Json(new
                {
                    success = true,
                    score = grade.Score.ToString("F1"), // Định dạng điểm với 1 số thập phân
                    gradeDate = grade.GradeDate.ToString("dd/MM/yyyy HH:mm"), // Định dạng ngày giờ
                    message = "Điểm đã được cập nhật thành công" // Thông báo thành công
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); // Trả về lỗi nếu có ngoại lệ
            }
        }

        public class GradeUpdateModel // Model để cập nhật điểm
        {
            public string CourseId { get; set; } // Mã khóa học
            public string Username { get; set; } // Username của sinh viên
            public decimal Score { get; set; } // Điểm số
        }

        public class GradeDistribution // Phân phối điểm
        {
            public int Excellent { get; set; } // Số lượng điểm >= 90
            public int Good { get; set; }      // Số lượng điểm >= 70
            public int AverageGrade { get; set; }   // Số lượng điểm >= 50
            public int Poor { get; set; }      // Số lượng điểm < 50
        }

        public IActionResult GradeStatistics()
        {
            var courses = _csvService.GetFacultyCourses(User.Identity.Name); // Lấy danh sách khóa học của giảng viên
            var allGradeStats = new List<(Course Course, GradeDistribution Stats)>(); // Danh sách thống kê điểm

            foreach (var course in courses)
            {
                var students = _csvService.GetEnrolledStudents(course.CourseId); // Lấy danh sách sinh viên đã ghi danh
                var gradeStats = new GradeDistribution(); // Khởi tạo thống kê điểm

                foreach (var student in students)
                {
                    var studentGrades = _csvService.GetStudentGrades(student.Username, course.CourseId); // Lấy điểm của sinh viên
                    if (studentGrades.Any())
                    {
                        var grade = studentGrades.First().Score; // Lấy điểm số
                        if (grade >= 90) gradeStats.Excellent++; // Tăng số lượng điểm xuất sắc
                        else if (grade >= 70) gradeStats.Good++; // Tăng số lượng điểm tốt
                        else if (grade >= 50) gradeStats.AverageGrade++; // Tăng số lượng điểm trung bình
                        else gradeStats.Poor++; // Tăng số lượng điểm kém
                    }
                }

                allGradeStats.Add((course, gradeStats)); // Thêm thống kê vào danh sách
            }

            return View(allGradeStats); // Trả về view với danh sách thống kê
        }
    }
}
//dbl