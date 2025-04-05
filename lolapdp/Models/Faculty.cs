using Microsoft.AspNetCore.Mvc;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp Faculty đại diện cho thông tin giảng viên trong hệ thống
    /// </summary>
    public class Faculty
    {
        /// <summary>
        /// Tên đăng nhập của giảng viên
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của giảng viên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Địa chỉ email của giảng viên
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Danh sách các khóa học được phân công giảng dạy
        /// </summary>
        public List<Course> AssignedCourses { get; set; }

        /// <summary>
        /// Khởi tạo đối tượng Faculty với danh sách khóa học rỗng
        /// </summary>
        public Faculty()
        {
            AssignedCourses = new List<Course>();
        }
    }
}
//dbl