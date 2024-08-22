using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class UserRolesViewModel
    {
        public List<UserRoleViewModel> UserRoles { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string NewRole { get; set; }
    }
}