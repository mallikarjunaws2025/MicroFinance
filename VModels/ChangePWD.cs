using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApp.VModels
{
    public class ChangePWD
    {
        [DisplayName("User ID")]
        [Required(ErrorMessage = "Please Enter User ID")]
        public string UserID { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "New Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }

        public string IsSucess { get; set; }
    }
}