using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using lolapdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly CSVService _csvService;

        public class GradeDistribution
        {
            public int Excellent { get; set; } // >= 90
            public int Good { get; set; }      // >= 70
            public int AverageGrade { get; set; }   // >= 50
            public int Poor { get; set; }      // < 50
        }

        public AdminController(CSVService csvService)
        {
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
        }

        public IActionResult Index()
        {
            var users = _csvService.GetAllUsers();
            var courses = _csvService.GetAllCourses();
            ViewBag.Users = users;
            ViewBag.Courses = courses;
            return View();
        }

        public IActionResult ManageFaculty()
        {
            var facultyList = _csvService.GetAllUsers()
                .Where(u => u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Lấy số khóa học của mỗi giảng viên
            var facultyCourses = new Dictionary<string, int>();
            foreach (var faculty in facultyList)
            {
                var courses = _csvService.GetFacultyCourses(faculty.Username);
                facultyCourses[faculty.Username] = courses.Count;
            }
            ViewBag.FacultyCourses = facultyCourses;

            return View(facultyList);
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

                _csvService.AddUser(newFaculty);
                TempData["Message"] = "Giảng viên đã được thêm thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageFaculty");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFaculty(string username, string fullName, string email)
        {
            try
            {
                var faculty = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username && u.Role == "Faculty");

                if (faculty != null)
                {
                    faculty.FullName = fullName;
                    faculty.Email = email;
                    _csvService.UpdateUser(faculty);
                    TempData["Message"] = "Thông tin giảng viên đã được cập nhật";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageFaculty");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFaculty(string username)
        {
            try
            {
                var faculty = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username && u.Role == "Faculty");

                if (faculty != null)
                {
                    // Kiểm tra xem giảng viên có đang dạy khóa học nào không
                    var courses = _csvService.GetFacultyCourses(username);
                    if (courses.Any())
                    {
                        throw new Exception("Không thể xóa giảng viên đang giảng dạy khóa học");
                    }

                    _csvService.DeleteUser(username);
                    TempData["Message"] = "Giảng viên đã được xóa thành công";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageFaculty");
        }

        public IActionResult ManageUsers()
        {
            var users = _csvService.GetAllUsers();
            return View(users);
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

                _csvService.AddUser(newUser);
                TempData["Message"] = "Người dùng đã được thêm thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(string username, string role, string fullName, string email)
        {
            try
            {
                var user = _csvService.GetAllUsers()
                    .FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    user.Role = role;
                    user.FullName = fullName;
                    user.Email = email;
                    _csvService.UpdateUser(user);
                    TempData["Message"] = "Thông tin người dùng đã được cập nhật";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string username)
        {
            try
            {
                _csvService.DeleteUser(username);
                TempData["Message"] = "Người dùng đã được xóa thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageUsers");
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

                _csvService.AddCourse(newCourse);
                TempData["Message"] = "Khóa học đã được thêm thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AssignFaculty(string courseId, string facultyUsername)
        {
            try
            {
                _csvService.AssignFacultyToCourse(courseId, facultyUsername);
                TempData["Message"] = "Đã phân công giảng viên thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public IActionResult AddStudentToCourse(string studentUsername, string courseId)
        {
            try
            {
                _csvService.AddStudentToCourse(studentUsername, courseId);
                TempData["Message"] = "Đã thêm sinh viên vào khóa học thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
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
                    var students = _csvService.GetEnrolledStudents(course.CourseId);
                    var gradesForCourse = new List<(User Student, Grade Grade)>();

                    foreach (var student in students)
                    {
                        var studentGrades = _csvService.GetStudentGrades(student.Username, course.CourseId);
                        if (studentGrades.Any())
                        {
                            gradesForCourse.Add((student, studentGrades.First()));
                        }
                    }

                    if (gradesForCourse.Any())
                    {
                        courseGrades[course.CourseId] = gradesForCourse;
                    }
                }

                ViewBag.CourseGrades = courseGrades;
                return View(courses);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải điểm: {ex.Message}";
                return View(new List<Course>());
            }
        }

        public IActionResult GradeStatistics()
        {
            var courses = _csvService.GetAllCourses();
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