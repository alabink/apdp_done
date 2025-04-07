namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức xác thực đăng nhập
    /// </summary>
    public interface ILoginService
    {
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