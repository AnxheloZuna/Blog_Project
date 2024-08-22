using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Blog_Project.Models;

namespace Blog_Project
{
    public class RoleBaseProviders : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var context = new UNIVERSITY_MANAGEMENT_SYSTEMEntities())
            {
                var result = (from user in context.Users_Table
                              join userRole in context.UserRoles on user.UserId equals userRole.UserId
                              join role in context.UserRoleMappings on userRole.RoleId equals role.Id
                              where user.CreatedBy == username
                              select role.Role).ToArray();
                return result;

            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}