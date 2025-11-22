using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.VModels
{
    public class BranchViewModel
    {
        [DisplayName("Branch Code")]
        [Required(ErrorMessage = "Please Enter BranchCode")]
        public string BranchCode { get; set; }
        [DisplayName("Branch Name")]
        [Required(ErrorMessage = "Please Enter Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Branch Address")]
        [Required(ErrorMessage = "Please Enter Branch Address")]
        public string BranchAddress { get; set; }
        [DisplayName("City")]
        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }
        [DisplayName("State")]
        [Required(ErrorMessage = "Please select State")]
        public string State { get; set; }
        [DisplayName("State")]
        public int PinCode { get; set; }
        public int StaffId { get; set; }
        
        // Staff list for dropdown
        public List<SelectListItem> StaffList { get; set; }

        public int OpenDateday { get; set; }
        public int OpenDateMonth { get; set; }
        public int OpenDateYear { get; set; }

        public string IsSucess { get; set; }
    }
}