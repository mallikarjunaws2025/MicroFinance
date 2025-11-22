using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.VModels
{
    public class BranchWithManagerViewModel
    {
        public int BranchID { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string OpenDate { get; set; }
        public string BAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? PinCode { get; set; }
        public int? ManagerID { get; set; }
        public string ManagerName { get; set; }
        public bool HasManager => ManagerID.HasValue && !string.IsNullOrEmpty(ManagerName);
    }
}