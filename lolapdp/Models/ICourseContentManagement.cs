namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức quản lý nội dung khóa học
    /// </summary>
    public interface ICourseContentManagement
    {
        /// <summary>
        /// Lấy danh sách tất cả các khóa học
        /// </summary>
        /// <returns>Danh sách các khóa học</returns>
        List<Course> GetAllCourses();

        /// <summary>
        /// Lấy thông tin khóa học theo ID
        /// </summary>
        /// <param name="id">ID của khóa học</param>
        /// <returns>Đối tượng Course nếu tìm thấy</returns>
        Course GetCourseById(int id);

        /// <summary>
        /// Thêm khóa học mới
        /// </summary>
        /// <param name="course">Đối tượng Course cần thêm</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại</returns>
        bool AddCourse(Course course);

        /// <summary>
        /// Cập nhật nội dung khóa học
        /// </summary>
        /// <param name="course">Đối tượng Course cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateCourseContent(Course course);

        /// <summary>
        /// Xóa khóa học
        /// </summary>
        /// <param name="id">ID của khóa học cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        bool DeleteCourse(int id);

        /// <summary>
        /// Lấy danh sách khóa học theo môn học
        /// </summary>
        /// <param name="subject">Tên môn học</param>
        /// <returns>Danh sách các khóa học thuộc môn học</returns>
        List<Course> GetCoursesBySubject(string subject);

        /// <summary>
        /// Cập nhật trạng thái khóa học
        /// </summary>
        /// <param name="courseId">ID của khóa học</param>
        /// <param name="isActive">Trạng thái mới</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateCourseStatus(int courseId, bool isActive);
    }
}