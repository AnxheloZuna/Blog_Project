using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Blog_Project.Models;
using Blog_Project.Services;

namespace Blog_Project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UNIVERSITY_MANAGEMENT_SYSTEMEntities Blog_DatabaseEntities;

        public HomeController()
        {
            Blog_DatabaseEntities = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        }

        private UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();

        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;

                var user = dbContext.Users_Table.FirstOrDefault(u => u.CreatedBy == username);

                if (user != null)
                {
                    return user.UserId;
                }
            }

            return 0;
        }

        public ActionResult Index()
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return View();
        }
    }
}