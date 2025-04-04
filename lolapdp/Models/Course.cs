using System;
using System.ComponentModel.DataAnnotations;

namespace lolapdp.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Faculty { get; set; }
        public bool IsActive { get; set; } = true;

        public string CourseCode => CourseId;

        public static Course FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            return new Course
            {
                CourseId = values[0],
                CourseName = values[1],
                Description = values[2],
                Credits = int.Parse(values[3]),
                Faculty = values.Length > 4 ? values[4] : "",
                IsActive = values.Length > 5 ? bool.Parse(values[5]) : true
            };
        }

        public string ToCsv()
        {
            return $"{CourseId},{CourseName},{Description},{Credits},{Faculty},{IsActive}";
        }
    }
}
//dbl