namespace lolapdp.Models
{
    /// <summary>
    /// Lớp StudentCourse đại diện cho mối quan hệ giữa sinh viên và khóa học
    /// </summary>
    public class StudentCourse
    {
        /// <summary>
        /// ID duy nhất của bản ghi đăng ký
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID của sinh viên
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// ID của khóa học
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Ngày đăng ký khóa học
        /// </summary>
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Trạng thái đăng ký (true: đang học, false: đã hủy)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Thông tin sinh viên
        /// </summary>
        public virtual User Student { get; set; }

        /// <summary>
        /// Thông tin khóa học
        /// </summary>
        public virtual Course Course { get; set; }
    }
} 