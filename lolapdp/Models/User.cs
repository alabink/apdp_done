using System;
using System.Security.Cryptography;
using System.Text;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp User đại diện cho thông tin người dùng trong hệ thống
    /// </summary>
    public class User
    {
        /// <summary>
        /// ID duy nhất của người dùng
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập của người dùng
        /// </summary>
        public string Username { get; set; }

        private string _password;

        /// <summary>
        /// Mật khẩu của người dùng (khi set sẽ tự động hash mật khẩu)
        /// </summary>
        public string Password 
        { 
            get => _password;
            set
            {
                _password = value;
                PasswordHash = HashPassword(value);
            }
        }

        /// <summary>
        /// Mật khẩu đã được hash
        /// </summary>
        public string PasswordHash { get; private set; }

        /// <summary>
        /// Vai trò của người dùng (Admin, Faculty, Student)
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Địa chỉ email của người dùng
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Tạo đối tượng User từ chuỗi CSV
        /// </summary>
        /// <param name="csvLine">Chuỗi CSV chứa thông tin người dùng</param>
        /// <returns>Đối tượng User được tạo từ chuỗi CSV</returns>
        public static User FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            var user = new User
            {
                Username = values[0],
                Role = values[2],
                FullName = values[3],
                Email = values[4]
            };
            user.Password = values[1]; // This will also set the PasswordHash
            return user;
        }

        /// <summary>
        /// Chuyển đổi đối tượng User thành chuỗi CSV
        /// </summary>
        /// <returns>Chuỗi CSV chứa thông tin người dùng</returns>
        public string ToCsv()
        {
            return $"{Username},{Password},{Role},{FullName},{Email}";
        }

        /// <summary>
        /// Hàm băm mật khẩu sử dụng SHA256
        /// </summary>
        /// <param name="password">Mật khẩu cần băm</param>
        /// <returns>Chuỗi mật khẩu đã được băm</returns>
        private static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
//dbl