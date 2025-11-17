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
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User ID must be between 3 and 50 characters")]
        public string UserID { get; set; }
        
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}