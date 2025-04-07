namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức quản lý thông tin cá nhân người dùng
    /// </summary>
    public interface IUserProfileManagement
    {
        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Đối tượng User nếu tìm thấy</returns>
        User GetUserById(int id);

        /// <summary>
        /// Lấy thông tin người dùng theo tên đăng nhập
        /// </summary>
        /// <param name="username">Tên đăng nhập của người dùng</param>
        /// <returns>Đối tượng User nếu tìm thấy</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Cập nhật thông tin cá nhân người dùng
        /// </summary>
        /// <param name="user">Đối tượng User cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateUserProfile(User user);

        /// <summary>
        /// Lấy thông tin email của người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Email của người dùng</returns>
        string GetUserEmail(int userId);

        /// <summary>
        /// Cập nhật email của người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="newEmail">Email mới</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateUserEmail(int userId, string newEmail);
    }
}