namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức xác thực người dùng
    /// </summary>
    public interface IAuthentication
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
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Đối tượng User nếu đăng nhập thành công, null nếu thất bại</returns>
        User? Login(string username, string password);

        /// <summary>
        /// Lấy vai trò của người dùng
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <returns>Vai trò của người dùng nếu tìm thấy, null nếu không tìm thấy</returns>
        string? GetUserRole(string username);

        /// <summary>
        /// Băm mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu cần băm</param>
        /// <returns>Chuỗi mật khẩu đã được băm</returns>
        string HashPassword(string password);
    }
}
//dbl