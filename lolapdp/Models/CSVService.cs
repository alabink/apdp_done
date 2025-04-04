using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace lolapdp.Models
{
    public class CSVService : ICSVService
    {
        private readonly string _dataDirectory;
        private readonly string _usersFile;
        private readonly string _coursesFile;
        private readonly string _enrollmentsFile;
        private readonly string _gradesFile;

        public CSVService()
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
            _usersFile = Path.Combine(_dataDirectory, "users.csv");
            _coursesFile = Path.Combine(_dataDirectory, "courses.csv");
            _enrollmentsFile = Path.Combine(_dataDirectory, "enrollments.csv");
            _gradesFile = Path.Combine(_dataDirectory, "grades.csv");
            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            // Create users.csv if not exists
            if (!File.Exists(_usersFile))
            {
                File.WriteAllText(_usersFile, "Username,Password,Role,FullName,Email\n");
            }

            // Create courses.csv if not exists
            if (!File.Exists(_coursesFile))
            {
                File.WriteAllText(_coursesFile, "CourseId,CourseName,Description,Credits,Faculty,IsActive\n");
            }

            // Create enrollments.csv if not exists
            if (!File.Exists(_enrollmentsFile))
            {
                File.WriteAllText(_enrollmentsFile, "Username,CourseId,EnrollmentDate\n");
            }

            // Create grades.csv if not exists
            if (!File.Exists(_gradesFile))
            {
                File.WriteAllText(_gradesFile, "Username,CourseId,Grade,GradeDate\n");
            }
        }

        public List<T> ReadCSV<T>(string filePath) where T : class, new()
        {
            var results = new List<T>();
            if (!File.Exists(filePath))
                return results;

            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1) 
                return results;

            var headers = lines[0].Split(',');
            var properties = typeof(T).GetProperties();

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var item = new T();

                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    var property = properties.FirstOrDefault(p => 
                        p.Name.Equals(headers[j], StringComparison.OrdinalIgnoreCase));
                    
                    if (property != null && !string.IsNullOrEmpty(values[j]))
                    {
                        var value = Convert.ChangeType(values[j], property.PropertyType);
                        property.SetValue(item, value);
                    }
                }

                results.Add(item);
            }

            return results;
        }

        public void WriteCSV<T>(string filePath, List<T> data) where T : class
        {
            var properties = typeof(T).GetProperties();
            var headers = string.Join(",", properties.Select(p => p.Name));
            var lines = new List<string> { headers };

            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                lines.Add(string.Join(",", values));
            }

            File.WriteAllLines(filePath, lines);
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            if (!File.Exists(_usersFile))
            {
                return users;
            }

            try
            {
                // Đọc tất cả các dòng từ file
                var lines = File.ReadAllLines(_usersFile, Encoding.UTF8);
                if (lines.Length <= 1)
                    return users;

                // Bỏ qua dòng header và xử lý từng dòng
                foreach (var line in lines.Skip(1))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            users.Add(new User
                            {
                                Username = parts[0].Trim(),
                                Password = parts[1].Trim(),
                                Role = parts[2].Trim(),
                                FullName = parts[3].Trim(),
                                Email = parts[4].Trim()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error reading users file: {ex.Message}");
            }

            return users;
        }

        public void AddUser(User user)
        {
            try
            {
                // Ensure data directory exists
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                // Create users.csv if not exists
                if (!File.Exists(_usersFile))
                {
                    File.WriteAllText(_usersFile, "Username,Password,Role,FullName,Email\n", Encoding.UTF8);
                }

                // Check if user already exists
                var users = GetAllUsers();
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Username already exists");
                }

                // Prepare new user data
                var line = $"{user.Username},{user.Password},{user.Role},{user.FullName},{user.Email}\n";
                
                // Write data to file
                File.AppendAllText(_usersFile, line, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}");
            }
        }

        public List<Course> GetAllCourses()
        {
            try
            {
                var courses = new List<Course>();
                
                if (!File.Exists(_coursesFile))
                {
                    File.WriteAllText(_coursesFile, "CourseId,CourseName,Description,Credits,Faculty,IsActive\n", Encoding.UTF8);
                    return courses;
                }

                var lines = File.ReadAllLines(_coursesFile, Encoding.UTF8);
                if (lines.Length <= 1)
                {
                    return courses;
                }

                foreach (var line in lines.Skip(1))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 6)
                        {
                            courses.Add(new Course
                            {
                                CourseId = parts[0],
                                CourseName = parts[1],
                                Description = parts[2],
                                Credits = int.Parse(parts[3]),
                                Faculty = parts[4],
                                IsActive = bool.Parse(parts[5])
                            });
                        }
                    }
                }
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc danh sách khóa học: {ex.Message}");
            }
        }

        public void AddCourse(Course course)
        {
            try
            {
                // Ensure data directory exists
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                // Create courses.csv if not exists
                if (!File.Exists(_coursesFile))
                {
                    File.WriteAllText(_coursesFile, "CourseId,CourseName,Description,Credits,Faculty,IsActive\n", Encoding.UTF8);
                }

                // Check if course already exists
                var courses = GetAllCourses();
                if (courses.Any(c => c.CourseId.Equals(course.CourseId, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception("Course ID already exists");
                }

                // Prepare new course data
                var line = $"{course.CourseId},{course.CourseName},{course.Description},{course.Credits},{course.Faculty},{course.IsActive}\n";
                
                // Write data to file
                File.AppendAllText(_coursesFile, line, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding course: {ex.Message}");
            }
        }

        public void UpdateCourse(Course course)
        {
            var courses = GetAllCourses();
            var index = courses.FindIndex(c => c.CourseId.Equals(course.CourseId, StringComparison.OrdinalIgnoreCase));
            
            if (index == -1)
            {
                throw new Exception("Course not found");
            }

            courses[index] = course;

            // Write all data back to file
            var lines = new List<string> { "CourseId,CourseName,Description,Credits,Faculty,IsActive" };
            foreach (var c in courses)
            {
                lines.Add($"{c.CourseId},{c.CourseName},{c.Description},{c.Credits},{c.Faculty},{c.IsActive}");
            }
            File.WriteAllLines(_coursesFile, lines, Encoding.UTF8);
        }

        public void AssignFacultyToCourse(string courseId, string facultyUsername)
        {
            var course = GetAllCourses().FirstOrDefault(c => 
                c.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase));

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            var faculty = GetAllUsers().FirstOrDefault(u => 
                u.Username.Equals(facultyUsername, StringComparison.OrdinalIgnoreCase) && 
                u.Role.Equals("Faculty", StringComparison.OrdinalIgnoreCase));

            if (faculty == null)
            {
                throw new Exception("Faculty not found");
            }

            course.Faculty = facultyUsername;
            UpdateCourse(course);
        }

        public List<Course> GetFacultyCourses(string facultyUsername)
        {
            return GetAllCourses()
                .Where(c => c.Faculty != null && 
                    c.Faculty.Equals(facultyUsername, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Course> GetStudentCourses(string username)
        {
            if (!File.Exists(_enrollmentsFile))
                return new List<Course>();

            try
            {
                var enrolledCourseIds = File.ReadAllLines(_enrollmentsFile, Encoding.UTF8)
                    .Skip(1)
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Select(line => line.Split(','))
                    .Where(parts => parts.Length >= 2 &&
                        parts[0].Trim().Equals(username, StringComparison.OrdinalIgnoreCase))
                    .Select(parts => parts[1].Trim())
                    .ToList();

                return GetAllCourses()
                    .Where(c => enrolledCourseIds.Contains(c.CourseId))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading student courses: {ex.Message}");
                return new List<Course>();
            }
        }

        public List<Grade> GetStudentGrades(string username)
        {
            return GetAllGrades()
                .Where(g => g.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Grade> GetStudentGrades(string username, string courseId)
        {
            return GetAllGrades()
                .Where(g => g.Username.Equals(username, StringComparison.OrdinalIgnoreCase) 
                       && g.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void AddGrade(Grade grade)
        {
            try
            {
                // Ensure data directory exists
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                // Create grades.csv if not exists
                if (!File.Exists(_gradesFile))
                {
                    File.WriteAllText(_gradesFile, "Username,CourseId,Score,GradeDate\n", Encoding.UTF8);
                }

                var lines = File.ReadAllLines(_gradesFile, Encoding.UTF8).ToList();
                
                // Remove old grade if exists
                lines = lines.Where(line =>
                {
                    if (string.IsNullOrEmpty(line) || line.Trim() == "Username,CourseId,Score,GradeDate")
                        return true;

                    var parts = line.Split(',');
                    return !(parts.Length >= 2 &&
                        parts[0].Trim().Equals(grade.Username, StringComparison.OrdinalIgnoreCase) &&
                        parts[1].Trim().Equals(grade.CourseId, StringComparison.OrdinalIgnoreCase));
                }).ToList();

                // Add new grade
                var line = $"{grade.Username},{grade.CourseId},{grade.Score},{grade.GradeDate:yyyy-MM-dd HH:mm:ss}\n";
                lines.Add(line);

                // Write all data back to file
                File.WriteAllLines(_gradesFile, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding grade: {ex.Message}");
            }
        }

        public List<User> GetEnrolledStudents(string courseId)
        {
            if (!File.Exists(_enrollmentsFile))
                return new List<User>();

            try
            {
                var enrolledUsernames = File.ReadAllLines(_enrollmentsFile, Encoding.UTF8)
                    .Skip(1)
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Select(line => line.Split(','))
                    .Where(parts => parts.Length >= 2 && parts[1].Trim().Equals(courseId, StringComparison.OrdinalIgnoreCase))
                    .Select(parts => parts[0].Trim())
                    .ToList();

                return GetAllUsers()
                    .Where(u => u.Role.Equals("Student", StringComparison.OrdinalIgnoreCase) &&
                        enrolledUsernames.Contains(u.Username))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading enrollments: {ex.Message}");
                return new List<User>();
            }
        }

        public void AddStudentToCourse(string username, string courseId)
        {
            try
            {
                // Ensure data directory exists
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                // Create enrollments.csv if not exists
                if (!File.Exists(_enrollmentsFile))
                {
                    File.WriteAllText(_enrollmentsFile, "Username,CourseId,EnrollmentDate\n", Encoding.UTF8);
                }

                // Check if student exists
                var user = GetAllUsers().FirstOrDefault(u => 
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                    u.Role.Equals("Student", StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    throw new Exception("Student not found");
                }

                // Check if course exists
                var course = GetAllCourses().FirstOrDefault(c => 
                    c.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase));

                if (course == null)
                {
                    throw new Exception("Course not found");
                }

                // Check if student is already enrolled
                var enrollments = File.ReadAllLines(_enrollmentsFile, Encoding.UTF8);
                if (enrollments.Skip(1).Any(line => 
                {
                    var parts = line.Split(',');
                    return parts.Length >= 2 && 
                        parts[0].Equals(username, StringComparison.OrdinalIgnoreCase) && 
                        parts[1].Equals(courseId, StringComparison.OrdinalIgnoreCase);
                }))
                {
                    throw new Exception("Student is already enrolled in this course");
                }

                // Add new enrollment
                var line = $"{username},{courseId},{DateTime.Now:yyyy-MM-dd}\n";
                File.AppendAllText(_enrollmentsFile, line, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error enrolling student: {ex.Message}");
            }
        }

        public void RemoveStudentFromCourse(string username, string courseId)
        {
            if (!File.Exists(_enrollmentsFile))
            {
                throw new Exception("Enrollment file not found");
            }

            var lines = File.ReadAllLines(_enrollmentsFile);
            var newLines = new List<string> { lines[0] }; // Keep header

            bool found = false;
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length >= 2 && 
                    parts[0].Equals(username, StringComparison.OrdinalIgnoreCase) && 
                    parts[1].Equals(courseId, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    continue; // Skip this line
                }
                newLines.Add(line);
            }

            if (!found)
            {
                throw new Exception("Student is not enrolled in this course");
            }

            File.WriteAllLines(_enrollmentsFile, newLines);
        }

        public User AuthenticateUser(string username, string password)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return null;
            }

            // Kiểm tra mật khẩu trực tiếp vì đang lưu plain text trong CSV
            if (!user.Password.Equals(password))
            {
                return null;
            }

            return user;
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }

            // Tạm thời bỏ qua hash password vì đang lưu plain text
            return password;
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            // So sánh trực tiếp vì đang lưu plain text
            return password.Equals(storedPassword);
        }

        public void UpdateUser(User user)
        {
            var users = GetAllUsers();
            var index = users.FindIndex(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
            
            if (index == -1)
            {
                throw new Exception("User not found");
            }

            users[index] = user;

            // Write all data back to file
            var lines = new List<string> { "Username,Password,Role,FullName,Email" };
            foreach (var u in users)
            {
                lines.Add($"{u.Username},{u.Password},{u.Role},{u.FullName},{u.Email}");
            }
            File.WriteAllLines(_usersFile, lines, Encoding.UTF8);
        }

        public void DeleteUser(string username)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
            {
                throw new Exception("User not found");
            }

            users.Remove(user);

            // Write all data back to file
            var lines = new List<string> { "Username,Password,Role,FullName,Email" };
            foreach (var u in users)
            {
                lines.Add($"{u.Username},{u.Password},{u.Role},{u.FullName},{u.Email}");
            }
            File.WriteAllLines(_usersFile, lines, Encoding.UTF8);
        }

        public List<Grade> GetAllGrades()
        {
            var grades = new List<Grade>();
            if (!File.Exists(_gradesFile))
            {
                return grades;
            }

            try
            {
                var lines = File.ReadAllLines(_gradesFile, Encoding.UTF8);
                if (lines.Length <= 1)
                    return grades;

                foreach (var line in lines.Skip(1))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var parts = line.Split(',');
                        if (parts.Length >= 4)
                        {
                            try
                            {
                                grades.Add(new Grade
                                {
                                    Username = parts[0].Trim(),
                                    CourseId = parts[1].Trim(),
                                    Score = decimal.Parse(parts[2].Trim()),
                                    GradeDate = DateTime.ParseExact(parts[3].Trim(), "yyyy-MM-dd HH:mm:ss", null)
                                });
                            }
                            catch
                            {
                                // Skip invalid lines
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading grades file: {ex.Message}");
            }

            return grades;
        }
    }
}
//dbl