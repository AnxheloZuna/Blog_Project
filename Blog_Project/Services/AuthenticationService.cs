using System;
using System.Net;
using System.Net.Mail;
using System.Linq;
using BCrypt.Net;
using Blog_Project.Models;
using System.Web.Security;
using System.Collections.Generic;

namespace Blog_Project.Services
{
    public class AuthenticationService
    {
        private UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext;

        public AuthenticationService()
        {
            dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        }

        public int AuthenticateUser(LoginViewModel credentials)
        {
            Users_Table user = findByUserName(credentials.Username);

            if (user != null)
            {

                string storedCombinedPasswordSalt = user.Password;
                string[] parts = storedCombinedPasswordSalt.Split(':');
                string storedHashedPassword = parts[0];

                if (BCrypt.Net.BCrypt.Verify(credentials.Password, storedHashedPassword))
                {
                    return 1;
                }
                else return 2;
            }
            else return 3;
        }

        public Users_Table findByUserName(string userName)
        {
            return dbContext.Users_Table.FirstOrDefault(x => x.CreatedBy == userName);
        }

        public bool Signup(Users_Table userinfo, UserRole role)
        {
            bool isUsernameInUse = dbContext.Users_Table.Any(u => u.CreatedBy == userinfo.CreatedBy);
            bool isEmailInUse = dbContext.Users_Table.Any(u => u.Email == userinfo.Email);

            if (isUsernameInUse)
            {
                return false;
            }
            if (isEmailInUse)
            {
                return false;
            }

            if (userinfo.Password != userinfo.ConfirmPassword)
            {
                return false;
            }

            if (string.IsNullOrEmpty(userinfo.Password) || userinfo.Password.Length < 8 || !HasLetter(userinfo.Password))
            {
                return false;
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userinfo.Password, salt);
            string combinedPasswordSalt = hashedPassword + ":" + salt;
            userinfo.Password = combinedPasswordSalt;
            userinfo.ConfirmPassword = combinedPasswordSalt;
            userinfo.CreatedAt = DateTime.Now;

            dbContext.Users_Table.Add(userinfo);
            dbContext.SaveChanges();

            int perdoruesRoleId = 2;
            role.RoleId = perdoruesRoleId;
            role.UserId = userinfo.UserId;
            dbContext.UserRoles.Add(role);
            dbContext.SaveChanges();

            return true;
        }

        private bool HasLetter(string value)
        {
            return value.Any(char.IsLetter);
        }

        public void SendVerificationEmail(string toEmail, string token)
        {
            string fromEmail = "serenaa.aruci@gmail.com";
            string fromPassword = "missyvjlketugqra";
            string host = "smtp.gmail.com";
            int port = 587; // or 465 for SSL
            bool enableSsl = true; // true for TLS, false for SSL

            string subject = "Email Verification";
            string body = $"Click the following link to verify your email:\n\nhttps://localhost:44335/Account/ConfirmEmail?token={token}";

            MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);

            using (SmtpClient smtpClient = new SmtpClient(host, port))
            {
                smtpClient.EnableSsl = enableSsl;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtpClient.Send(mail);
            }
        }

        public bool ConfirmEmail(string token)
        {
            var user = dbContext.Users_Table.FirstOrDefault(u => u.EmailConfirmationToken == token);
            if (user != null)
            {
                user.EmailConfirmed = true;
                //user.EmailConfirmationToken = null; // You can decide whether to nullify the token here or elsewhere.

                dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public List<UserRolesViewModel> GetUserRoles()
        {
            var roles = dbContext.UserRoleMappings.Select(role => role.Role).Distinct().ToList();

            var query = from user in dbContext.Users_Table
                        join userRole in dbContext.UserRoles on user.UserId equals userRole.UserId
                        join role in dbContext.UserRoleMappings on userRole.RoleId equals role.Id
                        select new UserRolesViewModel
                        {
                            UserId = user.UserId.ToString(),
                            UserName = user.CreatedBy,
                            Role = role.Role
                        };

            var results = query.ToList();
            var userRoles = roles.ToList();
            return results;
        }

        public List<string> GetDistinctRoles()
        {
            var roles = dbContext.UserRoleMappings.Select(role => role.Role).Distinct().ToList();
            return roles;
        }

        public int ChangeUserRole(int UserId, string NewRole, Users_Table updatedUser)
        {
            var userRole = dbContext.UserRoles.FirstOrDefault(u => u.UserId == UserId);

            if (userRole != null)
            {
                var roleMapping = dbContext.UserRoleMappings.FirstOrDefault(r => r.Role == NewRole);
                var role = dbContext.UserRoles.FirstOrDefault(user => user.RoleId == roleMapping.Id);
                Users_Table existingUser = dbContext.Users_Table.Find(UserId);

                if (roleMapping != null)
                {
                    userRole.RoleId = roleMapping.Id;
                    //role.ModifiedAt = DateTime.Now;
                    existingUser.ModifiedBy = updatedUser.ModifiedBy; // Update ModifiedBy using updatedUser's ModifiedBy
                    dbContext.SaveChanges();
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 3;
            }
        }


        public int AddRole(string newRole)
        {

            using (var dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities())
            {
                var roleMapping = new UserRoleMapping { Role = newRole };
                dbContext.UserRoleMappings.Add(roleMapping);
                bool isUsernameInUse = dbContext.UserRoleMappings.Any(u => u.Role == roleMapping.Role);

                if (isUsernameInUse)
                {
                    return 1;
                }
                else
                {
                    roleMapping.CreatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                    return 2;
                }
            }
        }
        public List<UserRolesViewModel> SearchUsers(string searchTerm)
        {
            var query = from user in dbContext.Users_Table
                        join userRole in dbContext.UserRoles on user.UserId equals userRole.UserId
                        join role in dbContext.UserRoleMappings on userRole.RoleId equals role.Id
                        where user.CreatedBy.Contains(searchTerm) || role.Role.Contains(searchTerm)
                        select new UserRolesViewModel
                        {
                            UserId = user.UserId.ToString(),
                            UserName = user.CreatedBy,
                            Role = role.Role
                        };
            var results = query.ToList();
            var matchingUser = results.Where(p => p.UserName.Contains(searchTerm)).ToList();
            return matchingUser;
        }

        public List<string> GetAvailableRoles()
        {
            return dbContext.UserRoleMappings.Select(role => role.Role).ToList();
        }
    }
}

