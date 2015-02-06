using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListApp.Web.UI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter a user name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
    }
}