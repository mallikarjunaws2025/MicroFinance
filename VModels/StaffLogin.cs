using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.VModels
{
    public class StaffLogin
    {
        public int UsrId { get; set; }
        public Nullable<int> StaffId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RolePermission { get; set; }
        public Nullable<int> IsLocked { get; set; }
    }
}