using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Blog_Project.Models;
using System.Linq;
using BCrypt.Net;
using System.Net.Mail;
using System.Net;
using Blog_Project.Services;

namespace Blog_Project.Controllers
{
    public class AccountController : Controller
    {
        AuthenticationService authenticationService = new AuthenticationService();

        UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }

        #region Login
        [HttpPost]
        public ActionResult Login(LoginViewModel credentials)
        {
            var userId = dbContext.Users_Table
                                .Where(x => x.CreatedBy == credentials.Username)
                                .Select(x => x.UserId)
                                .FirstOrDefault();
            var roleId = dbContext.UserRoles.Where(x => x.Id == userId).Select(x => x.RoleId).FirstOrDefault();
            var role = dbContext.UserRoleMappings.Where(x => x.Id == roleId).Select(x => x.Role).FirstOrDefault();

            switch (authenticationService.AuthenticateUser(credentials))
            {
                case 1:
                    FormsAuthentication.SetAuthCookie(credentials.Username, false);
                    if (role == "Admin")
                    {
                        return RedirectToAction("UserRoles", "Account");
                    }
                    else if (role == "Professor")
                    {
                        return RedirectToAction("Index", "ProfessorView");
                    }
                    else if (role == "Student")
                    {
                        return RedirectToAction("StudentIndex", "StudentView");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");

                    }
                case 2:
                    ModelState.AddModelError("", "Emri ose Fjalekalimi i pasakte!");
                    break;
                default: break;

            }
            return View();

        }
        #endregion

        #region Signup
        [HttpPost]
        public ActionResult Signup(Users_Table userinfo, UserRole role)
        {
            if (authenticationService.Signup(userinfo, role))
            {
                TempData["PostMessage"] = "Registration was completed successfully!";
                return Redirect("https://localhost:44335/Account/Login?ReturnUrl=%2f");
            }
            bool isUsernameInUse = dbContext.Users_Table.Any(u => u.CreatedBy == userinfo.CreatedBy);
            bool isEmailInUse = dbContext.Users_Table.Any(u => u.Email == userinfo.Email);

            if (isUsernameInUse)
            {
                ModelState.AddModelError("", "The name is in use. Please choose another name..");
                return View(userinfo);
            }
            if (isEmailInUse)
            {
                ModelState.AddModelError("", "Email is in use. Please choose another email.");
                return View(userinfo);
            }

            if (userinfo.Password != userinfo.ConfirmPassword)
            {
                ModelState.AddModelError("", "Password and Confirm password are not the same.");
                return View(userinfo);
            }

            else
            {
                ModelState.AddModelError("Password", "The password must be at least 8 characters long and must contain at least one letter.");
                return View(userinfo);
            }

        }

        #endregion

        #region UserRoles
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles()
        {
            var roles = authenticationService.GetDistinctRoles();
            var results = authenticationService.GetUserRoles();

            ViewBag.Roles = roles;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return View(results);
        }
        #endregion

        #region AddRole
        [Authorize(Roles = "Admin")]
        public ActionResult AddRole(string newRole)
        {

            switch (authenticationService.AddRole(newRole))
            {
                case 1:
                    TempData["AddRoleFailed"] = "Roli ekziston.";
                    break;
                case 2:
                    TempData["AddRole"] = "Roli u shtua me sukses!";
                    break;

            }
            return RedirectToAction("UserRoles");
        }
        #endregion

        #region ChangeUserRole
        [HttpPost]
        public ActionResult ChangeUserRole(int UserId, string NewRole, Users_Table updatedUser)
        {
            try
            {
                switch (authenticationService.ChangeUserRole(UserId, NewRole, updatedUser))
                {
                    case 1:
                        TempData["PostMessage"] = "Role changed successfully!";
                        break;
                    case 2:
                        TempData["PostMessage"] = "Role not found!";
                        break;
                    case 3:
                        TempData["PostMessage"] = "User not found!";
                        break;
                    default: break;
                }

                return RedirectToAction("UserRoles", "Account");
            }
            catch (Exception)
            {
                TempData["PostMessage"] = "An error occurred while changing the role.";
                return RedirectToAction("UserRoles", "Account");
            }
        }


        #endregion

        #region SearchUser
        [HttpPost]
        public ActionResult SearchUser(string searchTerm)
        {
            var userPosts = authenticationService.SearchUsers(searchTerm);

            ViewBag.Roles = authenticationService.GetAvailableRoles();

            return View("UserRoles", userPosts.ToList());
        }
        #endregion
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            return RedirectToAction("Login");
        }

    }
}