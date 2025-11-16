using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestApp.VModels.Reports
{
    public class SummeryReport
    {
        public string SlNo { get; set; }
        public string Particulars { get; set; }
        public string BOR { get; set; }
        public string Added { get; set; }
        public string Dropped { get; set; }
        public string EOR { get; set; }

        public string Receipts { get; set; }
        public string RRs { get; set; }

        public string Payments { get; set; }
        public string PRs { get; set; }
    }
}