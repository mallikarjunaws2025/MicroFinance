using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.VModels.Member
{
    public class MemberViewModel
    {
        [Required(ErrorMessage = "Please Enter Member Name")]
        public string MbrName { get; set; }

        [Required(ErrorMessage = "Please select Group Name")]
        public string GrpCode { get; set; }
        public string HsbndName { get; set; }
        [Required(ErrorMessage = "Please Enter Age")]
        public int Age { get; set; }
        public string MbrDOJ { get; set; }
        [Required(ErrorMessage = "Please select member status")]
        public string MbrStatus { get; set; } // Depend on out stand Amt
        public string Withdrawdate { get; set; }
        public int StaffID { get; set; }
        [Required(ErrorMessage = "Please Enter Member Address")]
        public string MbrAddress { get; set; }

        [Required(ErrorMessage = "Please select Gender")]
        public string Gen { get; set; }
        public string CantactNum { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Invalid")]
        [RegularExpression("^[0-9]*$", ErrorMessage = " Adhara Card Num must be numeric")]
        public string AdharaCardNum { get; set; }
        public string RationCardNum { get; set; }
        public string VoterIDNum { get; set; }
        public string Nominee { get; set; }
        public string PhoneNo2 { get; set; }

        public List<SelectListItem> CrDateday { get; set; }
        public List<SelectListItem> CrDateMonth { get; set; }
        public List<SelectListItem> CrDateYear { get; set; }

        public string CrD { get; set; }
        public string CrM { get; set; }
        public string CrY { get; set; }

        public List<SelectListItem> WDateday { get; set; }
        public List<SelectListItem> WDateMonth { get; set; }
        public List<SelectListItem> WDateYear { get; set; }

        public string WD { get; set; }
        public string WM { get; set; }
        public string WY { get; set; }
        public string IsSucess { get; set; }
        public List<SelectListItem> GrpNameList { get; set; }
        public List<SelectListItem> StaffMbrList { get; set; }

        public string GrpName { get; set; }
        public string StaffName { get; set; }

        public string ALRSavings { get; set; }

        public int MbrID { get; set; }
        
            
    }
}