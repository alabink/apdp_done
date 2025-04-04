using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitTest.Models;

namespace UnitTest.Services
{
    public class CSVService
    {
        private readonly string _dataPath;

        public CSVService(string dataPath)
        {
            _dataPath = dataPath;
        }

        public User GetUser(string username)
        {
            var lines = File.ReadAllLines(Path.Combine(_dataPath, "users.csv")).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts[0] == username)
                {
                    return new User
                    {
                        Username = parts[0],
                        Password = parts[1],
                        Role = parts[2],
                        FullName = parts[3],
                        Email = parts[4]
                    };
                }
            }
            return null;
        }

        public List<Grade> GetStudentGrades(string username)
        {
            var lines = File.ReadAllLines(Path.Combine(_dataPath, "grades.csv")).Skip(1);
            var grades = new List<Grade>();
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts[0] == username)
                {
                    grades.Add(new Grade
                    {
                        Username = parts[0],
                        CourseId = parts[1],
                        Score = int.Parse(parts[2]),
                        GradeDate = DateTime.Parse(parts[3])
                    });
                }
            }
            return grades;
        }

        public List<Grade> GetStudentGrades(string username, string courseId)
        {
            return GetStudentGrades(username).Where(g => g.CourseId == courseId).ToList();
        }

        public List<Course> GetCourses()
        {
            var lines = File.ReadAllLines(Path.Combine(_dataPath, "courses.csv")).Skip(1);
            var courses = new List<Course>();
            foreach (var line in lines)
            {
                var parts = line.Split(',');
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
            return courses;
        }

        public List<Enrollment> GetEnrollments()
        {
            var lines = File.ReadAllLines(Path.Combine(_dataPath, "enrollments.csv")).Skip(1);
            var enrollments = new List<Enrollment>();
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                enrollments.Add(new Enrollment
                {
                    Username = parts[0],
                    CourseId = parts[1],
                    EnrollmentDate = DateTime.Parse(parts[2])
                });
            }
            return enrollments;
        }
    }
} 