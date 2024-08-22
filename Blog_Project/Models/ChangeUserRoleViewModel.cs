using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class ChangeUserRoleViewModel
    {
        public int UserId { get; set; }
        public int NewRoleId { get; set; }
    }

}