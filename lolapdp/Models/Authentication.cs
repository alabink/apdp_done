using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp Authentication thực hiện các chức năng xác thực người dùng
    /// </summary>
    public class Authentication : IAuthentication
    {
        /// <summary>
        /// Danh sách người dùng trong hệ thống
        /// </summary>
        private readonly IEnumerable<User> _users;

        /// <summary>
        /// Khởi tạo đối tượng Authentication với danh sách người dùng
        /// </summary>
        /// <param name="users">Danh sách người dùng</param>
        public Authentication(IEnumerable<User> users)
        {
            _users = users;
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="role">Vai trò của người dùng</param>
        /// <returns>True nếu đăng ký thành công, False nếu thất bại</returns>
        public bool Register(string username, string password, string role)
        {
            if (_users.Any(u => u.Username == username))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Đối tượng User nếu đăng nhập thành công, null nếu thất bại</returns>
        public User? Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        /// <summary>
        /// Lấy vai trò của người dùng
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <returns>Vai trò của người dùng nếu tìm thấy, null nếu không tìm thấy</returns>
        public string? GetUserRole(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return user?.Role;
        }

        /// <summary>
        /// Băm mật khẩu sử dụng SHA256
        /// </summary>
        /// <param name="password">Mật khẩu cần băm</param>
        /// <returns>Chuỗi mật khẩu đã được băm</returns>
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Xác minh mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <param name="passwordHash">Mật khẩu đã được băm</param>
        /// <returns>True nếu mật khẩu khớp, False nếu không khớp</returns>
        private bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }
    }
}
//dbl