using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApp.VModels
{
    public class StaffSignIn
    {
        [DisplayName("User ID")]
        [Required(ErrorMessage = "Please Enter User ID")]
        public string UserID { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }

    }
}