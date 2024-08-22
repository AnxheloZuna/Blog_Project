using Blog_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class StudentVM
    {
        public string isEvaluatedTitle { get; set; }
        public string Evaluation_Time { get; set; }
        public string evaluationReview { get; set; }
        public string evaluationRetake { get; set; }
        public List<StuentsDataViewModel> Students { get; set; }
    }

    public class StuentsDataViewModel
    {
        public string CourseTitle { get; set; }
        public DateTime? ExamDate { get; set; }
        public Decimal? Grade { get; set; }
        public int? Absent { get; set; }
        public string ProfessorName { get; set; }
        public string isEvaluatedTitle { get; set; }
        public string Evaluation_Time { get; set; }
        public string evaluationReview { get; set; }
        public string evaluationRetake { get; set; }
    }


}