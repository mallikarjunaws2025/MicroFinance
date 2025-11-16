using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.VModels.Group
{
    public class Groups
    {
        [DisplayName("Group Code")]
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string RecoveryDay { get; set; }
        [DisplayName("Village")]
        [Required(ErrorMessage = "Please Enter Village ")]
        public string Village { get; set; }
        [DisplayName("TotalMember")]
        public int TotalMember { get; set; }
        [DisplayName("Group Created Date")]
        public string CrDate { get; set; }
        [DisplayName("Date Of Collection")]
        public string DOC { get; set; }
        [DisplayName("Date Of Collection")]
        public int Distance { get; set; }
        [DisplayName("Group Status")]
        [Required(ErrorMessage = "Please Select Group Status")]
        public string Status { get; set; }
        [DisplayName("Staff ID")]
        [Required(ErrorMessage = "Please Select Staff ID")]
        public int StaffID { get; set; }
        [DisplayName("Formed By")]
        [Required(ErrorMessage = "Please Select Formed By Name")]
        public string FormedBy { get; set; }
        [DisplayName("Meeting Place Address")]
        [Required(ErrorMessage = "Please Enter MeetingPlaceAddress")]
        public string MeetingPlaceAddress { get; set; }
        [DisplayName("Group Type")]
        [Required(ErrorMessage = "Please Select Group Type")]
        public string GroupType { get; set; }

        public int CrD { get; set; }
        public int CrM { get; set; }
        public int CrY { get; set; }


        public int CCrD { get; set; }
        public int CCrM { get; set; }
        public int CCrY { get; set; }

        public List<System.Web.Mvc.SelectListItem> BranchCodeList { get; set; }
        public List<System.Web.Mvc.SelectListItem> StaffMbrList { get; set; }
        public string IsSucess { get; set; }
    }
}