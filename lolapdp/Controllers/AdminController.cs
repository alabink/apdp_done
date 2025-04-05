using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ cho phép người dùng có vai trò Admin truy cập
    public class AdminController : Controller
    {
        private readonly CSVService _csvService; // Dịch vụ CSV để thao tác với dữ liệu

        public class GradeDistribution
        {
            public int Excellent { get; set; } // Số lượng điểm >= 90
            public int Good { get; set; }      // Số lượng điểm >= 70
            public int AverageGrade { get; set; }   // Số lượng điểm >= 50
            public int Poor { get; set; }      // Số lượng điểm < 50
        }

        public AdminController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService)); // Khởi tạo dịch vụ CSV
        }

        public IActionResult Index()
        {
            var users = _csvService.GetAllUsers(); // Lấy tất cả người dùng
            var courses = _csvService.GetAllCourses(); // Lấy tất cả khóa học
            ViewBag.Users = users; // Lưu người dùng vào ViewBag
            ViewBag.Courses = courses; // Lưu khóa học vào ViewBag
            return View(); // Trả về view
        }

        public IActionResult ManageFaculty()
        {
            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList(); // Lấy danh sách giảng viên

            // Lấy số khóa học của mỗi giảng viên
            var facultyCourses = new Dictionary<string, int>();
            foreach (var faculty in facultyList)
            {
                var courses = _csvService.GetFacultyCourses(faculty.Username); // Lấy khóa học của giảng viên
                facultyCourses[faculty.Username] = courses.Count; // Đếm số khóa học
            }
            ViewBag.FacultyCourses = facultyCourses; // Lưu số khóa học vào ViewBag

            return View(facultyList); // Trả về view với danh sách giảng viên
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFaculty(string username, string password, string fullName, string email)
        {
            try
            {
                var newFaculty = new User
                {
                    Username = username,
                    Password = password,
                    Role = "Faculty",
                    FullName = fullName,
                    Email = email
                };

                _csvService.AddUser(newFaculty); // Thêm giảng viên mới
                TempData["Message"] = "Giảng viên đã được thêm thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageFaculty"); // Chuyển hướng về trang quản lý giảng viên
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFaculty(string username, string fullName, string email)
        {
            try
            {
                var faculty = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username && u.Role == "Faculty"); // Tìm giảng viên theo username

                if (faculty != null)
                {
                    faculty.FullName = fullName; // Cập nhật tên đầy đủ
                    faculty.Email = email; // Cập nhật email
                    _csvService.UpdateUser(faculty); // Cập nhật thông tin giảng viên
                    TempData["Message"] = "Thông tin giảng viên đã được cập nhật"; // Thông báo thành công
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageFaculty"); // Chuyển hướng về trang quản lý giảng viên
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFaculty(string username)
        {
            try
            {
                var faculty = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username && u.Role == "Faculty"); // Tìm giảng viên theo username

                if (faculty != null)
                {
                    // Kiểm tra xem giảng viên có đang dạy khóa học nào không
                    var courses = _csvService.GetFacultyCourses(username);
                    if (courses.Any())
                    {
                        throw new Exception("Không thể xóa giảng viên đang giảng dạy khóa học"); // Ném lỗi nếu giảng viên đang dạy
                    }

                    _csvService.DeleteUser(username); // Xóa giảng viên
                    TempData["Message"] = "Giảng viên đã được xóa thành công"; // Thông báo thành công
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageFaculty"); // Chuyển hướng về trang quản lý giảng viên
        }

        public IActionResult ManageUsers()
        {
            var users = _csvService.GetAllUsers(); // Lấy tất cả người dùng
            return View(users); // Trả về view với danh sách người dùng
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(string username, string password, string role, string fullName, string email)
        {
            try
            {
                var newUser = new User
                {
                    Username = username,
                    Password = password,
                    Role = role,
                    FullName = fullName,
                    Email = email
                };

                _csvService.AddUser(newUser); // Thêm người dùng mới
                TempData["Message"] = "Người dùng đã được thêm thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageUsers"); // Chuyển hướng về trang quản lý người dùng
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(string username, string role, string fullName, string email)
        {
            try
            {
                var user = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username); // Tìm người dùng theo username

                if (user != null)
                {
                    user.Role = role; // Cập nhật vai trò
                    user.FullName = fullName; // Cập nhật tên đầy đủ
                    user.Email = email; // Cập nhật email
                    _csvService.UpdateUser(user); // Cập nhật thông tin người dùng
                    TempData["Message"] = "Thông tin người dùng đã được cập nhật"; // Thông báo thành công
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageUsers"); // Chuyển hướng về trang quản lý người dùng
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string username)
        {
            try
            {
                _csvService.DeleteUser(username); // Xóa người dùng
                TempData["Message"] = "Người dùng đã được xóa thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageUsers"); // Chuyển hướng về trang quản lý người dùng
        }

        [HttpPost]
        public IActionResult AddCourse(string courseId, string courseName, string description, int credits)
        {
            try
            {
                var newCourse = new Course
                {
                    CourseId = courseId,
                    CourseName = courseName,
                    Description = description,
                    Credits = credits
                };

                _csvService.AddCourse(newCourse); // Thêm khóa học mới
                TempData["Message"] = "Khóa học đã được thêm thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("Index"); // Chuyển hướng về trang chính
        }

        [HttpPost]
        public IActionResult AssignFaculty(string courseId, string facultyUsername)
        {
            try
            {
                _csvService.AssignFacultyToCourse(courseId, facultyUsername); // Phân công giảng viên cho khóa học
                TempData["Message"] = "Đã phân công giảng viên thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("ManageUsers"); // Chuyển hướng về trang quản lý người dùng
        }

        [HttpPost]
        public IActionResult AddStudentToCourse(string studentUsername, string courseId)
        {
            try
            {
                _csvService.AddStudentToCourse(studentUsername, courseId); // Thêm sinh viên vào khóa học
                TempData["Message"] = "Đã thêm sinh viên vào khóa học thành công"; // Thông báo thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Thông báo lỗi
            }
            return RedirectToAction("Index"); // Chuyển hướng về trang chính
        }

        public IActionResult ViewGrades()
        {
            try
            {
                // Lấy danh sách tất cả khóa học và sắp xếp
                var courses = _csvService.GetAllCourses()
                    .OrderBy(c => c.CourseId)
                    .ToList();

                var courseGrades = new Dictionary<string, List<(User Student, Grade Grade)>>();

                foreach (var course in courses)
                {
                    var students = _csvService.GetEnrolledStudents(course.CourseId); // Lấy danh sách sinh viên đã ghi danh
                    var gradesForCourse = new List<(User Student, Grade Grade)>();

                    foreach (var student in students)
                    {
                        var studentGrades = _csvService.GetStudentGrades(student.Username, course.CourseId); // Lấy điểm của sinh viên
                        if (studentGrades.Any())
                        {
                            gradesForCourse.Add((student, studentGrades.First())); // Thêm điểm vào danh sách
                        }
                    }

                    if (gradesForCourse.Any())
                    {
                        courseGrades[course.CourseId] = gradesForCourse; // Lưu điểm của khóa học vào dictionary
                    }
                }

                ViewBag.CourseGrades = courseGrades; // Lưu điểm của các khóa học vào ViewBag
                return View(courses); // Trả về view với danh sách khóa học
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải điểm: {ex.Message}"; // Thông báo lỗi
                return View(new List<Course>()); // Trả về view rỗng nếu có lỗi
            }
        }

        public IActionResult GradeStatistics()
        {
            var courses = _csvService.GetAllCourses(); // Lấy tất cả khóa học
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