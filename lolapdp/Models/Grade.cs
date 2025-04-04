using System;

namespace lolapdp.Models
{
    public class Grade
    {
        public string Username { get; set; }
        public string CourseId { get; set; }
        public decimal Score { get; set; }
        public DateTime GradeDate { get; set; }

        public string ToCsv()
        {
            return $"{Username},{CourseId},{Score},{GradeDate:yyyy-MM-dd}";
        }

        public static Grade FromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            return new Grade
            {
                Username = values[0],
                CourseId = values[1],
                Score = decimal.Parse(values[2]),
                GradeDate = DateTime.Parse(values[3])
            };
        }
    }
} 