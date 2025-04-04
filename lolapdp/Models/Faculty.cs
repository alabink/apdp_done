using Microsoft.AspNetCore.Mvc;

namespace lolapdp.Models
{
    public class Faculty
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<Course> AssignedCourses { get; set; }

        public Faculty()
        {
            AssignedCourses = new List<Course>();
        }
    }
}
//dbl