using System;
using System.Collections.Generic;
using System.Linq;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp UserManagement thực hiện các chức năng quản lý người dùng
    /// </summary>
    public class UserManagement : IUserManagement
    {
        /// <summary>
        /// Danh sách người dùng trong hệ thống
        /// </summary>
        private List<User> _users;

        /// <summary>
        /// Dịch vụ đọc/ghi file CSV
        /// </summary>
        private readonly ICSVService _csvService;

        /// <summary>
        /// Dịch vụ xác thực người dùng
        /// </summary>
        

        /// <summary>
        /// Khởi tạo đối tượng UserManagement
        /// </summary>
        /// <param name="csvService">Dịch vụ đọc/ghi file CSV</param>
        /// <param name="authentication">Dịch vụ xác thực người dùng</param>
        public UserManagement(ICSVService csvService)
        {
            _csvService = csvService;

            _users = new List<User>();
            LoadUsers();
        }

        /// <summary>
        /// Tải danh sách người dùng từ file CSV
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                _users = _csvService.ReadCSV<User>("Data/users.csv");
            }
            catch (Exception)
            {
                _users = new List<User>();
            }
        }

        /// <summary>
        /// Lưu danh sách người dùng vào file CSV
        /// </summary>
        private void SaveUsers()
        {
            try
            {
                _csvService.WriteCSV("Data/users.csv", _users);
            }
            catch (Exception)
            {
                // Log exception
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách các người dùng</returns>
        public List<User> GetAllUsers()
        {
            return _users.ToList();
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Đối tượng User nếu tìm thấy</returns>
        public User GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo tên đăng nhập
        /// </summary>
        /// <param name="username">Tên đăng nhập của người dùng</param>
        /// <returns>Đối tượng User nếu tìm thấy</returns>
        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        /// <summary>
        /// Thêm người dùng mới
        /// </summary>
        /// <param name="user">Đối tượng User cần thêm</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại</returns>
        public bool AddUser(User user)
        {
            if (_users.Any(u => u.Id == user.Id || u.Username == user.Username))
                return false;

            _users.Add(user);
            SaveUsers();
            return true;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="user">Đối tượng User cần cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu thất bại</returns>
        public bool UpdateUser(User user)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                return false;

            _users[index] = user;
            SaveUsers();
            return true;
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa</param>
        /// <returns>True nếu xóa thành công, False nếu thất bại</returns>
        public bool DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return false;

            _users.Remove(user);
            SaveUsers();
            return true;
        }
    }
} 