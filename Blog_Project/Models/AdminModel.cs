using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class AdminModel
    {
        public class Admin
        {
            public int AdminId { get; set; }
            public string AdminUsername { get; set; }
            public string AdminPassword { get; set; }
            public string AdminEmail { get; set; }
            public DateTime DateTime { get; set; }
        }

    }
}