using Blog_Project.Models;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;

namespace Blog_Project.Services
{
    public class AdminService
    {

        private UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext;

        public AdminService()
        {
            dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();

        }

        public int GetCurrentUserIdByName(string name)
        {
            var user = dbContext.Users_Table.FirstOrDefault(u => u.CreatedBy == name);

            return user.UserId;

        }
    }
}
