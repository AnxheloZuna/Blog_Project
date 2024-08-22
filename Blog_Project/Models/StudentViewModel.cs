using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class StudentViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal? Grade { get; set; }
        public int COURSES_ID { get; set; }
        public int PROFESSOR_ID { get; set; }
        public int? Absent {  get; set; }

    }
}