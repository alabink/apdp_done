using System;

namespace lolapdp.Models
{
    /// <summary>
    /// Lớp triển khai các dịch vụ xác thực theo mẫu Singleton
    /// </summary>
    public class AuthenticationService : ILoginService, IRegistrationService
    {
        private static AuthenticationService? _instance;
        private static readonly object _lock = new object();
        private readonly IUserManagement _userManagement;

        private AuthenticationService(IUserManagement userManagement)
        {
            _userManagement = userManagement;
        }

        /// <summary>
        /// Lấy instance duy nhất của AuthenticationService
        /// </summary>
        public static AuthenticationService GetInstance(IUserManagement userManagement)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AuthenticationService(userManagement);
                    }
                }
            }
            return _instance;
        }

        public User? Login(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var users = _userManagement.GetAllUsers();
            return users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);
        }

        public string? GetUserRole(string username)
        {
            var users = _userManagement.GetAllUsers();
            return users.FirstOrDefault(u => u.Username == username)?.Role;
        }

        public bool Register(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var users = _userManagement.GetAllUsers();
            if (users.Any(u => u.Username == username))
                return false;

            var hashedPassword = HashPassword(password);
            var newUser = new User
            {
                Username = username,
                Password = hashedPassword,
                Role = role
            };

            return _userManagement.AddUser(newUser);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}