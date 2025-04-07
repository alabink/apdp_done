namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức đăng ký người dùng
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="role">Vai trò của người dùng</param>
        /// <returns>True nếu đăng ký thành công, False nếu thất bại</returns>
        bool Register(string username, string password, string role);

        /// <summary>
        /// Băm mật khẩu cho người dùng mới
        /// </summary>
        /// <param name="password">Mật khẩu cần băm</param>
        /// <returns>Chuỗi mật khẩu đã được băm</returns>
        string HashPassword(string password);
    }
}