using System;
using System.Collections.Generic;
using System.Linq;
using Blog_Project.Models;
using BCrypt.Net;
using System.Web.Security;
using System.Web.ModelBinding;

namespace Blog_Project.Services
{
    public class ProfileService
    {
        private UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext;

        public ProfileService()
        {
            dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        }

        public bool UpdateUserProfile(Users_Table updatedUser)
        {

            Users_Table existingUser = dbContext.Users_Table.Find(updatedUser.UserId);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.CreatedBy = updatedUser.CreatedBy;
            existingUser.Email = updatedUser.Email;
            existingUser.ModifiedAt = DateTime.Now;
            existingUser.ModifiedBy = updatedUser.UserId;

            bool isUsernameInUse = dbContext.Users_Table.Any(u => u.CreatedBy == updatedUser.CreatedBy && u.UserId != updatedUser.UserId);

            if (isUsernameInUse)
            {
                return false;
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password, salt);
            existingUser.Password = hashedPassword;
            existingUser.ConfirmPassword = hashedPassword;

            dbContext.SaveChanges();

            return true;
        }

    }
}
