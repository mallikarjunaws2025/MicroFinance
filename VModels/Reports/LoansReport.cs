using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestApp.DB;

namespace TestApp.VModels.Reports
{
    public class LoansReport
    {
        public List<StaffNames> StaffName { get; set; } 
    }

    public class StaffNames
    {
        public string StaffName { get; set; }
        public List<GrpNames> GrpName = new List<GrpNames>();
    }

    public class GrpNames
    {
        public string GrpName { get; set; }
        public List<MbrLists> MbrList = new List<MbrLists>();
    }

    public class MbrLists
    {
        public spLoansReport_Result MbrList = new spLoansReport_Result();
    }
}