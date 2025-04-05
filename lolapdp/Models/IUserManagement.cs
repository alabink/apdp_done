namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức quản lý người dùng
    /// </summary>
    public interface IUserManagement
    {
        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách các người dùng</returns>
        List<User> GetAllUsers();

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
        /// Thêm người dùng mới
        /// </summary>
        /// <param name="user">Đối tượng User cần thêm</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại</returns>
        bool AddUser(User user);

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="user">Đối tượng User cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateUser(User user);

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        bool DeleteUser(int id);
    }
}
//dbl