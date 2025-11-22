using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.VModels
{
    public class StaffWithRoleViewModel
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string Status { get; set; }
        public string RolePermission { get; set; }
        public string ContactNum { get; set; } // Contact number from Staff.ContactNumber field
        public bool HasLogin { get; set; }
    }
}