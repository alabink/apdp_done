using System;
using System.ComponentModel.DataAnnotations;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp Course đại diện cho một khóa học trong hệ thống
    /// </summary>
    public class Course
    {
        /// <summary>
        /// ID duy nhất của khóa học
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mã khóa học
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Tên khóa học
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Mô tả về khóa học
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Số tín chỉ của khóa học
        /// </summary>
        public int Credits { get; set; }

        /// <summary>
        /// Tên đăng nhập của giảng viên phụ trách
        /// </summary>
        public string Faculty { get; set; }

        /// <summary>
        /// Trạng thái hoạt động của khóa học (true: đang hoạt động, false: đã đóng)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Mã khóa học (alias cho CourseId)
        /// </summary>
        public string CourseCode => CourseId;

        /// <summary>
        /// Tạo đối tượng Course từ chuỗi CSV
        /// </summary>
        /// <param name="csvLine">Chuỗi CSV chứa thông tin khóa học</param>
        /// <returns>Đối tượng Course được tạo từ chuỗi CSV</returns>
        public static Course FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            return new Course
            {
                CourseId = values[0],
                CourseName = values[1],
                Description = values[2],
                Credits = int.Parse(values[3]),
                Faculty = values.Length > 4 ? values[4] : "",
                IsActive = values.Length > 5 ? bool.Parse(values[5]) : true
            };
        }

        /// <summary>
        /// Chuyển đổi đối tượng Course thành chuỗi CSV
        /// </summary>
        /// <returns>Chuỗi CSV chứa thông tin khóa học</returns>
        public string ToCsv()
        {
            return $"{CourseId},{CourseName},{Description},{Credits},{Faculty},{IsActive}";
        }
    }
}
//dbl