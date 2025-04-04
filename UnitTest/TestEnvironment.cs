using Xunit;
using System;
using System.IO;

namespace UnitTest
{
    public class TestEnvironment
    {
        private readonly string _testDataPath;

        public TestEnvironment()
        {
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
            SetupTestEnvironment();
        }

        private void SetupTestEnvironment()
        {
            if (Directory.Exists(_testDataPath))
            {
                Directory.Delete(_testDataPath, true);
            }
            Directory.CreateDirectory(_testDataPath);

            // Create users.csv
            File.WriteAllText(Path.Combine(_testDataPath, "users.csv"),
                "Username,Password,Role,FullName,Email\n" +
                "admin,admin123,Admin,Administrator,admin@example.com\n" +
                "faculty1,faculty123,Faculty,Faculty One,faculty1@example.com\n" +
                "student1,student123,Student,Student One,student1@example.com\n" +
                "student2,student123,Student,Student Two,student2@example.com");

            // Create courses.csv
            File.WriteAllText(Path.Combine(_testDataPath, "courses.csv"),
                "CourseId,CourseName,Description,Credits,Faculty,IsActive\n" +
                "7436,Test Course 1,Description 1,3,faculty1,true\n" +
                "7007,Test Course 2,Description 2,4,faculty1,true");

            // Create enrollments.csv
            File.WriteAllText(Path.Combine(_testDataPath, "enrollments.csv"),
                "Username,CourseId,EnrollmentDate\n" +
                "student1,7436,2024-04-04\n" +
                "student2,7436,2024-04-04");

            // Create grades.csv
            File.WriteAllText(Path.Combine(_testDataPath, "grades.csv"),
                "Username,CourseId,Score,GradeDate\n" +
                "student1,7436,95,2024-04-04 11:02:00\n" +
                "student2,7436,85,2024-04-04 11:05:00");
        }

        [Fact]
        public void TestEnvironment_IsProperlyConfigured()
        {
            // Arrange & Act
            SetupTestEnvironment();

            // Assert
            Assert.True(Directory.Exists(_testDataPath), "TestData directory should exist");
            Assert.True(File.Exists(Path.Combine(_testDataPath, "users.csv")), "users.csv should exist");
            Assert.True(File.Exists(Path.Combine(_testDataPath, "courses.csv")), "courses.csv should exist");
            Assert.True(File.Exists(Path.Combine(_testDataPath, "enrollments.csv")), "enrollments.csv should exist");
            Assert.True(File.Exists(Path.Combine(_testDataPath, "grades.csv")), "grades.csv should exist");
        }
    }
} 