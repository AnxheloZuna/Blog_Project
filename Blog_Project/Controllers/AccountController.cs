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
            switch (authenticationService.AuthenticateUser(credentials))
            {
                case 1:
                    FormsAuthentication.SetAuthCookie(credentials.Username, false);
                    return RedirectToAction("Index", "Home");
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
                TempData["PostMessage"] = "Regjistrimi u krye me sukses!";
                return Redirect("https://localhost:44335/Account/Login?ReturnUrl=%2f");
            }
            bool isUsernameInUse = dbContext.Users_Table.Any(u => u.CreatedBy == userinfo.CreatedBy);
            bool isEmailInUse = dbContext.Users_Table.Any(u => u.Email == userinfo.Email);

            if (isUsernameInUse)
            {
                ModelState.AddModelError("", "Emri eshte ne perdorim. Ju lutem zgjidhni nje emer tjeter.");
                return View(userinfo);
            }
            if (isEmailInUse)
            {
                ModelState.AddModelError("", "Email-i eshte ne perdorim. Ju lutem zgjidhni nje email tjeter.");
                return View(userinfo);
            }

            if (userinfo.Password != userinfo.ConfirmPassword)
            {
                ModelState.AddModelError("", "Fjalekalimi dhe Konfirmo fjalekalimin nuk jane te njejte.");
                return View(userinfo);
            }

            else
            {
                ModelState.AddModelError("Password", "Fjalekalimi duhet te jete te pakten me 8 karaktere dhe duhet te permbaje te pakten nje germe.");
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
                        TempData["PostMessage"] = "Roli u ndryshua me sukses!";
                        break;
                    case 2:
                        TempData["PostMessage"] = "Roli nuk u gjet!";
                        break;
                    case 3:
                        TempData["PostMessage"] = "Përdoruesi nuk u gjet!";
                        break;
                    default: break;
                }

                return RedirectToAction("UserRoles", "Account");
            }
            catch (Exception)
            {
                TempData["PostMessage"] = "Ndodhi një gabim gjatë ndryshimit të rolit.";
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