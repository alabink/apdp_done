using System;
using System.Security.Cryptography;
using System.Text;

namespace lolapdp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        private string _password;
        public string Password 
        { 
            get => _password;
            set
            {
                _password = value;
                PasswordHash = HashPassword(value);
            }
        }
        public string PasswordHash { get; private set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

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

        public string ToCsv()
        {
            return $"{Username},{Password},{Role},{FullName},{Email}";
        }

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