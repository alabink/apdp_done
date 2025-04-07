namespace lolapdp.Models
{
    /// <summary>
    /// Interface định nghĩa các phương thức quản lý phân quyền người dùng
    /// </summary>
    public interface IUserAuthorizationManagement
    {
        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách các người dùng</returns>
        List<User> GetAllUsers();

        /// <summary>
        /// Thêm người dùng mới với quyền được chỉ định
        /// </summary>
        /// <param name="user">Đối tượng User cần thêm</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại</returns>
        bool AddUser(User user);

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        bool DeleteUser(int id);

        /// <summary>
        /// Cập nhật quyền của người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="newRole">Quyền mới</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        bool UpdateUserRole(int userId, string newRole);

        /// <summary>
        /// Kiểm tra quyền của người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="role">Quyền cần kiểm tra</param>
        /// <returns>True nếu người dùng có quyền, False nếu không</returns>
        bool CheckUserRole(int userId, string role);

        /// <summary>
        /// Lấy danh sách người dùng theo quyền
        /// </summary>
        /// <param name="role">Quyền cần lọc</param>
        /// <returns>Danh sách người dùng có quyền được chỉ định</returns>
        List<User> GetUsersByRole(string role);
    }
}