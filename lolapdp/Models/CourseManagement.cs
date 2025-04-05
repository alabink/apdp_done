using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp CourseManagement thực hiện các chức năng quản lý khóa học
    /// </summary>
    public class CourseManagement : ICourseManagement
    {
        /// <summary>
        /// Danh sách các khóa học
        /// </summary>
        private List<Course> _courses;

        /// <summary>
        /// Dịch vụ đọc/ghi file CSV
        /// </summary>
        private readonly ICSVService _csvService;

        /// <summary>
        /// Khởi tạo đối tượng CourseManagement
        /// </summary>
        /// <param name="csvService">Dịch vụ đọc/ghi file CSV</param>
        public CourseManagement(ICSVService csvService)
        {
            _csvService = csvService;
            _courses = new List<Course>();
            LoadCourses();
        }

        /// <summary>
        /// Tải danh sách khóa học từ file CSV
        /// </summary>
        private void LoadCourses()
        {
            try
            {
                _courses = _csvService.ReadCSV<Course>("Data/courses.csv");
            }
            catch (Exception)
            {
                _courses = new List<Course>();
            }
        }

        /// <summary>
        /// Lưu danh sách khóa học vào file CSV
        /// </summary>
        private void SaveCourses()
        {
            try
            {
                _csvService.WriteCSV("Data/courses.csv", _courses);
            }
            catch (Exception)
            {
                // Log exception
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các khóa học
        /// </summary>
        /// <returns>Danh sách các khóa học</returns>
        public List<Course> GetAllCourses()
        {
            return _courses.ToList();
        }

        /// <summary>
        /// Lấy thông tin khóa học theo ID
        /// </summary>
        /// <param name="id">ID của khóa học</param>
        /// <returns>Đối tượng Course nếu tìm thấy</returns>
        public Course GetCourseById(int id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Thêm khóa học mới
        /// </summary>
        /// <param name="course">Đối tượng Course cần thêm</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại</returns>
        public bool AddCourse(Course course)
        {
            if (_courses.Any(c => c.Id == course.Id))
                return false;

            _courses.Add(course);
            SaveCourses();
            return true;
        }

        /// <summary>
        /// Cập nhật thông tin khóa học
        /// </summary>
        /// <param name="course">Đối tượng Course cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        public bool UpdateCourse(Course course)
        {
            var index = _courses.FindIndex(c => c.Id == course.Id);
            if (index == -1)
                return false;

            _courses[index] = course;
            SaveCourses();
            return true;
        }

        /// <summary>
        /// Xóa khóa học
        /// </summary>
        /// <param name="id">ID của khóa học cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        public bool DeleteCourse(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
                return false;

            _courses.Remove(course);
            SaveCourses();
            return true;
        }
    }
} 