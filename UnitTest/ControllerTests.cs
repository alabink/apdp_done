using Xunit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using UnitTest.Services;
using UnitTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitTest
{
    public class ControllerTests : TestEnvironment, IDisposable
    {
        private readonly string _testDataPath;
        private readonly CSVService _csvService;

        public ControllerTests() : base()
        {
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
            Directory.CreateDirectory(_testDataPath);
            _csvService = new CSVService(_testDataPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDataPath))
            {
                Directory.Delete(_testDataPath, true);
            }
        }

        private class StudentController : Controller
        {
            private readonly CSVService _csvService;

            public StudentController(CSVService csvService)
            {
                _csvService = csvService;
            }

            public IActionResult Grades()
            {
                var username = HttpContext.Session.GetString("Username");
                var grades = _csvService.GetStudentGrades(username);
                return View(grades);
            }
        }

        private class FacultyController : Controller
        {
            private readonly CSVService _csvService;

            public FacultyController(CSVService csvService)
            {
                _csvService = csvService;
            }

            public IActionResult AssignGrade(string courseId)
            {
                var course = _csvService.GetCourses().Find(c => c.CourseId == courseId);
                var enrollments = _csvService.GetEnrollments().FindAll(e => e.CourseId == courseId);
                ViewData["Course"] = course;
                ViewData["Students"] = enrollments;
                return View();
            }

            public IActionResult UpdateGrade(string username, string courseId, int score)
            {
                // In a real application, this would update the grade in the CSV file
                return Json(new { success = true, score = score });
            }

            public IActionResult GradeStatistics()
            {
                var username = HttpContext.Session.GetString("Username");
                var courses = _csvService.GetCourses().FindAll(c => c.Faculty == username);
                var stats = new List<CourseGradeStatistics>();

                foreach (var course in courses)
                {
                    var enrollments = _csvService.GetEnrollments().FindAll(e => e.CourseId == course.CourseId);
                    var grades = new List<Grade>();
                    foreach (var enrollment in enrollments)
                    {
                        var studentGrades = _csvService.GetStudentGrades(enrollment.Username, course.CourseId);
                        grades.AddRange(studentGrades);
                    }

                    var stat = new CourseGradeStatistics
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        TotalStudents = enrollments.Count,
                        ExcellentCount = grades.Count(g => g.Score >= 90),
                        GoodCount = grades.Count(g => g.Score >= 70 && g.Score < 90),
                        AverageCount = grades.Count(g => g.Score >= 50 && g.Score < 70),
                        PoorCount = grades.Count(g => g.Score < 50)
                    };
                    stats.Add(stat);
                }

                return View(stats);
            }
        }

        [Fact]
        public void StudentController_Grades_ReturnsViewWithGrades()
        {
            // Arrange
            var controller = new StudentController(_csvService);
            var username = "student1";
            var testGrades = new List<Grade>
            {
                new Grade { Username = username, CourseId = "7436", Score = 100, GradeDate = DateTime.Parse("2024-04-04 11:02:00") },
                new Grade { Username = username, CourseId = "7007", Score = 20, GradeDate = DateTime.Parse("2024-04-04 14:07:00") }
            };

            File.WriteAllText(Path.Combine(_testDataPath, "grades.csv"),
                "Username,CourseId,Score,GradeDate\n" +
                $"{testGrades[0].Username},{testGrades[0].CourseId},{testGrades[0].Score},{testGrades[0].GradeDate}\n" +
                $"{testGrades[1].Username},{testGrades[1].CourseId},{testGrades[1].Score},{testGrades[1].GradeDate}");

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("Username", username);
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Grades() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as List<Grade>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void FacultyController_AssignGrade_ReturnsViewWithStudents()
        {
            // Arrange
            var controller = new FacultyController(_csvService);
            var courseId = "7436";
            var facultyUsername = "faculty1";

            // Create test data
            File.WriteAllText(Path.Combine(_testDataPath, "courses.csv"),
                "CourseId,CourseName,Description,Credits,Faculty,IsActive\n" +
                $"{courseId},Test Course,Description,3,{facultyUsername},true");

            File.WriteAllText(Path.Combine(_testDataPath, "enrollments.csv"),
                "Username,CourseId,EnrollmentDate\n" +
                "student1,7436,2024-04-04\n" +
                "student2,7436,2024-04-04");

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("Username", facultyUsername);
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.AssignGrade(courseId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ViewData["Students"]);
            Assert.NotNull(result.ViewData["Course"]);
        }

        [Fact]
        public void FacultyController_UpdateGrade_UpdatesGradeSuccessfully()
        {
            // Arrange
            var controller = new FacultyController(_csvService);
            var username = "student1";
            var courseId = "7436";
            var score = 95;

            // Create test data
            File.WriteAllText(Path.Combine(_testDataPath, "grades.csv"),
                "Username,CourseId,Score,GradeDate\n" +
                $"{username},{courseId},85,2024-04-04 11:02:00");

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("Username", "faculty1");
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.UpdateGrade(username, courseId, score) as JsonResult;

            // Assert
            Assert.NotNull(result);
            dynamic response = result.Value;
            Assert.True((bool)response.success);
            Assert.Equal(score, (int)response.score);
        }

        [Fact]
        public void FacultyController_GradeStatistics_ReturnsViewWithStatistics()
        {
            // Arrange
            var controller = new FacultyController(_csvService);
            var facultyUsername = "faculty1";
            var courseId = "7436";

            // Create test data
            File.WriteAllText(Path.Combine(_testDataPath, "courses.csv"),
                "CourseId,CourseName,Description,Credits,Faculty,IsActive\n" +
                $"{courseId},Test Course,Description,3,{facultyUsername},true");

            File.WriteAllText(Path.Combine(_testDataPath, "grades.csv"),
                "Username,CourseId,Score,GradeDate\n" +
                "student1,7436,95,2024-04-04 11:02:00\n" +
                "student2,7436,85,2024-04-04 11:05:00\n" +
                "student3,7436,75,2024-04-04 11:10:00\n" +
                "student4,7436,65,2024-04-04 11:15:00\n" +
                "student5,7436,45,2024-04-04 11:20:00");

            // Mock HttpContext
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("Username", facultyUsername);
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.GradeStatistics() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            var model = result.Model as List<CourseGradeStatistics>;
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal(5, model[0].TotalStudents);
        }
    }

    // Helper class for testing session
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionData = new Dictionary<string, byte[]>();

        public string Id => "TestSessionId";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionData.Keys;

        public void Clear()
        {
            _sessionData.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _sessionData.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _sessionData[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessionData.TryGetValue(key, out value);
        }

        public void SetString(string key, string value)
        {
            Set(key, System.Text.Encoding.UTF8.GetBytes(value));
        }

        public string GetString(string key)
        {
            if (TryGetValue(key, out byte[] value))
            {
                return System.Text.Encoding.UTF8.GetString(value);
            }
            return null;
        }
    }
} 