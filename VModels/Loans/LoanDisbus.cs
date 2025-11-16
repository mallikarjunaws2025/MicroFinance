using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.DB;

namespace TestApp.VModels.Loans
{
    public class LoanDisbus
    {
        public int LoanID { get; set; }
        public int MbrId { get; set; }
        public string Loan_Amount { get; set; }
        public string Balance_Amount { get; set; }
        public string Prin_EMI { get; set; }
        public string Int_EMI { get; set; }
        public string RateOfInterest { get; set; }
        public string ALRAmt { get; set; }
        public string Date_Of_Disbursement { get; set; }
        public string Expiry_Date { get; set; }
        public string NextDueDt { get; set; }        
        public string Processing_Fee { get; set; }
        public string GRFAmt { get; set; }
        public int Insurance { get; set; }
        public string Other_Income { get; set; }
        public string Savings { get; set; }
        public int NoOfDays { get; set; }
        public string IsSucess { get; set; }
        public int Stationary { get; set; }
        public string CrD { get; set; }
        public string CrM { get; set; }
        public string CrY { get; set; }

        public string AdvancedEMI { get; set; }

        public string LoanType { get; set; }

        public List<SelectListItem> Dateday { get; set; }
        public List<SelectListItem> DateMonth { get; set; }
        public List<SelectListItem> DateYear { get; set; }

        public IEnumerable<Loan> LoanDis { get; set; }

        public List<SelectListItem> GrpCodeList { get; set; }
        public List<SelectListItem> MbrList { get; set; }

        public string GType { get; set; }

        public string SelectedGrpName { get; set; }
        public string SelectedMbrName { get; set; }

        public string StaffName { get; set; }
        public string GrpName { get; set; }
    }
}