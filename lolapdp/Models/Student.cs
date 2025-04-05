namespace lolapdp.Models
{
    /// <summary>
    /// Lớp Student đại diện cho thông tin sinh viên trong hệ thống
    /// Lớp này hiện đang trống và có thể được mở rộng trong tương lai
    /// để thêm các thuộc tính và phương thức cụ thể cho sinh viên
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Tên đăng nhập của sinh viên
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của sinh viên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Email của sinh viên
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Mã số sinh viên
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// Ngày sinh của sinh viên
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính của sinh viên
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Số điện thoại của sinh viên
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Địa chỉ của sinh viên
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Danh sách các khóa học mà sinh viên đã đăng ký
        /// </summary>
        public List<Course> EnrolledCourses { get; set; }

        /// <summary>
        /// Danh sách điểm của sinh viên
        /// </summary>
        public List<Grade> Grades { get; set; }

        /// <summary>
        /// Khởi tạo một đối tượng Student mới
        /// </summary>
        public Student()
        {
            EnrolledCourses = new List<Course>();
            Grades = new List<Grade>();
        }

        /// <summary>
        /// Đăng ký một khóa học mới
        /// </summary>
        /// <param name="course">Khóa học cần đăng ký</param>
        public void EnrollInCourse(Course course)
        {
            if (!EnrolledCourses.Any(c => c.CourseId == course.CourseId))
            {
                EnrolledCourses.Add(course);
            }
        }

        /// <summary>
        /// Hủy đăng ký một khóa học
        /// </summary>
        /// <param name="courseId">Mã khóa học cần hủy đăng ký</param>
        public void UnenrollFromCourse(string courseId)
        {
            var course = EnrolledCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (course != null)
            {
                EnrolledCourses.Remove(course);
            }
        }

        /// <summary>
        /// Lấy điểm của một khóa học cụ thể
        /// </summary>
        /// <param name="courseId">Mã khóa học</param>
        /// <returns>Điểm của khóa học</returns>
        public Grade GetGradeForCourse(string courseId)
        {
            return Grades.FirstOrDefault(g => g.CourseId == courseId);
        }

        /// <summary>
        /// Tính điểm trung bình của tất cả các khóa học
        /// </summary>
        /// <returns>Điểm trung bình</returns>
        public double CalculateGPA()
        {
            if (Grades.Count == 0)
                return 0;

            return (double)Grades.Average(g => g.Score);
        }
    }
}
//dbl