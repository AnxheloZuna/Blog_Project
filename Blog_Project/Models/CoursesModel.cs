using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class CoursesModel
    {
        public int Id { get; set; }
        public int? ProfessorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExamDate { get; set; }
        public int StudentCount { get; set; }
        public int? Evaluated {get; set; }
        public string AnswersTime {get; set; }
        public int? Review {get; set; }
        public int? Retake {get; set; }
    }
}