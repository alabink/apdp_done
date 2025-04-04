using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnitTest.Services;
using UnitTest.Models;

namespace UnitTest
{
    public class CSVServiceTests : TestEnvironment, IDisposable
    {
        private readonly CSVService _csvService;
        private readonly string _testDataPath;

        public CSVServiceTests() : base()
        {
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
            _csvService = new CSVService(_testDataPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDataPath))
            {
                Directory.Delete(_testDataPath, true);
            }
        }

        [Fact]
        public void GetUser_ValidUsername_ReturnsUser()
        {
            // Act
            var result = _csvService.GetUser("student1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("student1", result.Username);
            Assert.Equal("student123", result.Password);
            Assert.Equal("Student", result.Role);
        }

        [Fact]
        public void GetUser_InvalidUsername_ReturnsNull()
        {
            // Act
            var result = _csvService.GetUser("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetStudentGrades_ValidUsername_ReturnsGrades()
        {
            // Act
            var result = _csvService.GetStudentGrades("student1");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(95, result[0].Score);
        }

        [Fact]
        public void GetStudentGrades_WithCourseFilter_ReturnsFilteredGrades()
        {
            // Act
            var result = _csvService.GetStudentGrades("student1", "7436");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("7436", result[0].CourseId);
            Assert.Equal(95, result[0].Score);
        }

        [Fact]
        public void GetCourses_ReturnsAllCourses()
        {
            // Act
            var result = _csvService.GetCourses();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Course 1", result[0].CourseName);
            Assert.Equal("Test Course 2", result[1].CourseName);
        }

        [Fact]
        public void GetEnrollments_ReturnsAllEnrollments()
        {
            // Act
            var result = _csvService.GetEnrollments();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("student1", result[0].Username);
            Assert.Equal("student2", result[1].Username);
        }
    }
} 