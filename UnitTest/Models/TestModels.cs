using System;

namespace UnitTest.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class Course
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Faculty { get; set; }
        public bool IsActive { get; set; }
    }

    public class Grade
    {
        public string Username { get; set; }
        public string CourseId { get; set; }
        public int Score { get; set; }
        public DateTime GradeDate { get; set; }
    }

    public class Enrollment
    {
        public string Username { get; set; }
        public string CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class CourseGradeStatistics
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public int TotalStudents { get; set; }
        public int ExcellentCount { get; set; }
        public int GoodCount { get; set; }
        public int AverageCount { get; set; }
        public int PoorCount { get; set; }
    }
} 