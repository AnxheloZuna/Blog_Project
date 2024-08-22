using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Blog_Project.Models;
using System.Linq;
using BCrypt.Net;
using System.Net;
using Blog_Project.Services;
using System.Web;

namespace Blog_Project.Controllers
{
    public class ProfileController : Controller
    {

        ProfileService profileService = new ProfileService();
        UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();

        public ProfileController()
        {
            dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            string currentUsername = User.Identity.Name;
            Users_Table user = dbContext.Users_Table.FirstOrDefault(u => u.CreatedBy == currentUsername);

            if (user == null)
            {
                return HttpNotFound();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(Users_Table updatedUser)
        {
            if (profileService.UpdateUserProfile(updatedUser))
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                FormsAuthentication.SignOut();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Profile update failed"); // Handle validation errors
            return View(updatedUser);
        }
    }
}