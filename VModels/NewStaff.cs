using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.VModels
{
    public class NewStaff
    {
        public string StaffName { get; set; }
        
        public string DOB { get; set; }
        public Int32 DOBD { get; set; }
         [Required(ErrorMessage = "Please Enter DOB")]
        public Int32 DOBM { get; set; }
         [Required(ErrorMessage = "Please Enter DOB")]
        public Int32 DOBY { get; set; }

        public string Status { get; set; }
        public string CantactNum { get; set; }
        public string RolePermission { get; set; }
        public string DOJ { get; set; }
         [Required(ErrorMessage = "Please Enter DOJ")]
        public Int32 DOJD { get; set; }
         [Required(ErrorMessage = "Please Enter DOJ")]
        public Int32 DOJM { get; set; }
         [Required(ErrorMessage = "Please Enter DOJ")]
        public Int32 DOJY { get; set; }

        public string IsSucess { get; set; }
    }
}