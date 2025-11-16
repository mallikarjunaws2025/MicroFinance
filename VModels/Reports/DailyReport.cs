using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.VModels.Reports
{
    public class DailyReport
    {
        public string SlNo;
        public string GroupName;
        public string NoOfClts;
        public string StartDt;
        public string EndDt;
        public string LA;
        public string LP;
        public string LI;
        public string TALRCollected;
        public string Days;

        public string Principal;
        public string Interest;
        public string ALRColl;
        public string Total;
        public string iTNoOfClts;
        public int LastCount;
        public string dHTotal , dTHLA , dTHLPA , dTHLI , dTHALRColl,
         dHAdvanceALRColl , dHAdvanceALRAdjusted , dTotalHNetCash , dTHPrin , dTHInt , dHTALRCollCur ;

    }
}