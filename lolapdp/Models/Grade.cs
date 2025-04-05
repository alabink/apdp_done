using System;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp Grade đại diện cho điểm số của sinh viên trong một khóa học
    /// </summary>
    public class Grade
    {
        /// <summary>
        /// Tên đăng nhập của sinh viên
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Mã khóa học
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Điểm số của sinh viên
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Ngày chấm điểm
        /// </summary>
        public DateTime GradeDate { get; set; }

        /// <summary>
        /// Chuyển đổi đối tượng Grade thành chuỗi CSV
        /// </summary>
        /// <returns>Chuỗi CSV chứa thông tin điểm</returns>
        public string ToCsv()
        {
            return $"{Username},{CourseId},{Score},{GradeDate:yyyy-MM-dd}";
        }

        /// <summary>
        /// Tạo đối tượng Grade từ chuỗi CSV
        /// </summary>
        /// <param name="csvLine">Chuỗi CSV chứa thông tin điểm</param>
        /// <returns>Đối tượng Grade được tạo từ chuỗi CSV</returns>
        public static Grade FromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            return new Grade
            {
                Username = values[0],
                CourseId = values[1],
                Score = decimal.Parse(values[2]),
                GradeDate = DateTime.Parse(values[3])
            };
        }
    }
} 