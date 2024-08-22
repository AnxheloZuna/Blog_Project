using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog_Project.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Emri i perdoruesit eshte i detyruar!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Fjalekalimi eshte i detyruar!")]
        public string Password { get; set; }
    }
}
