using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.DB;

namespace TestApp.VModels.Loans
{
    public class LoanDisbus
    {
        public int LoanID { get; set; }
        
        [Required(ErrorMessage = "Please select a member")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid member")]
        public int MbrId { get; set; }
        
        [Required(ErrorMessage = "Loan amount is required")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid loan amount")]
        public string Loan_Amount { get; set; }
        
        public string Balance_Amount { get; set; }
        
        [Required(ErrorMessage = "Principal EMI is required")]
        public string Prin_EMI { get; set; }
        
        public string Int_EMI { get; set; }
        
        [Required(ErrorMessage = "Interest amount is required")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid interest amount")]
        public string RateOfInterest { get; set; }
        
        public string ALRAmt { get; set; }
        
        [Required(ErrorMessage = "Disbursement date is required")]
        public string Date_Of_Disbursement { get; set; }
        
        [Required(ErrorMessage = "Expiry date is required")]
        public string Expiry_Date { get; set; }
        
        [Required(ErrorMessage = "Next due date is required")]
        public string NextDueDt { get; set; }        
        
        public string Processing_Fee { get; set; }
        public string GRFAmt { get; set; }
        public int Insurance { get; set; }
        public string Other_Income { get; set; }
        public string Savings { get; set; }
        
        [Required(ErrorMessage = "Number of days is required")]
        [Range(1, 365, ErrorMessage = "Please enter a valid number of days (1-365)")]
        public int NoOfDays { get; set; }
        
        public string IsSucess { get; set; }
        public int Stationary { get; set; }
        public string CrD { get; set; }
        public string CrM { get; set; }
        public string CrY { get; set; }

        public string AdvancedEMI { get; set; }

        [Required(ErrorMessage = "Please select a loan type")]
        public string LoanType { get; set; }

        public List<SelectListItem> Dateday { get; set; }
        public List<SelectListItem> DateMonth { get; set; }
        public List<SelectListItem> DateYear { get; set; }

        public IEnumerable<Loan> LoanDis { get; set; }

        public List<SelectListItem> GrpCodeList { get; set; }
        public List<SelectListItem> MbrList { get; set; }

        public string GType { get; set; }
        public string GrpCode { get; set; } // Selected group code

        public string SelectedGrpName { get; set; }
        public string SelectedMbrName { get; set; }

        public string StaffName { get; set; }
        public string GrpName { get; set; }
    }
}