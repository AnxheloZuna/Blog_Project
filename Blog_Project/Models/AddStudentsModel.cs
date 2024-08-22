using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class AddStudentsModel
    {
        public List<StudentViewModel> StudentsEnrolled { get; set; }

        public List<StudentViewModel> StudentsNotEnrolled { get; set; }

    }
}