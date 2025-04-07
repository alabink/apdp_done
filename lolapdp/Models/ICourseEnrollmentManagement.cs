namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức quản lý đăng ký khóa học
    /// </summary>
    public interface ICourseEnrollmentManagement
    {
        /// <summary>
        /// Đăng ký khóa học cho sinh viên
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của khóa học</param>
        /// <returns>True nếu đăng ký thành công, False nếu thất bại</returns>
        bool EnrollStudent(int studentId, int courseId);

        /// <summary>
        /// Hủy đăng ký khóa học của sinh viên
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của khóa học</param>
        /// <returns>True nếu hủy đăng ký thành công, False nếu thất bại</returns>
        bool UnenrollStudent(int studentId, int courseId);

        /// <summary>
        /// Lấy danh sách sinh viên đã đăng ký khóa học
        /// </summary>
        /// <param name="courseId">ID của khóa học</param>
        /// <returns>Danh sách sinh viên đã đăng ký</returns>
        List<Student> GetEnrolledStudents(int courseId);

        /// <summary>
        /// Lấy danh sách khóa học của sinh viên
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <returns>Danh sách khóa học đã đăng ký</returns>
        List<Course> GetStudentCourses(int studentId);

        /// <summary>
        /// Kiểm tra sinh viên đã đăng ký khóa học chưa
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="courseId">ID của khóa học</param>
        /// <returns>True nếu đã đăng ký, False nếu chưa</returns>
        bool IsStudentEnrolled(int studentId, int courseId);

        /// <summary>
        /// Lấy số lượng sinh viên đã đăng ký khóa học
        /// </summary>
        /// <param name="courseId">ID của khóa học</param>
        /// <returns>Số lượng sinh viên đã đăng ký</returns>
        int GetEnrollmentCount(int courseId);
    }
}