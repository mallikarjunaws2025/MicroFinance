using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApp.VModels.Loans
{
    public class LoanCols
    {
        public int LoanColId { get; set; }
        public int LoanId { get; set; }
        public int MbrId { get; set; }
        public string Collected_Amount { get; set; }
        public string Actual_Balance { get; set; }
        public string Prin_Balance { get; set; }
        public string Int_Collect { get; set; }
        public string Prin_Collected { get; set; }
        public int Balance_Installment { get; set; }
        public string Balance_Interest { get; set; }
        public string Prin_Due { get; set; }
        public string Int_Due { get; set; }
        public string Transact_Date { get; set; }
        public string Next_Due_Date { get; set; }
        public string Upto_Last_Savings { get; set; }
        public string During_Savings { get; set; }
        public string As_On { get; set; }
        public string Adjustment { get; set; }

        public string TD { get; set; }
        public string TM { get; set; }
        public string TY { get; set; }

        public string NxtDueD { get; set; }
        public string NxtDueM { get; set; }
        public string NxtDueY { get; set; }
        public string IsSucess { get; set; }
        public bool IsPrepaid { get; set; }
        public string PrepaidLoanIds { get; set; }
        
        public string SrchQry { get; set; }
        public string GrpCode { get; set; }
        public List<SelectListItem> GrpCodeList { get; set; }
        public List<SelectListItem> StaffMbrList { get; set; }

        public string GType { get; set; }
        
    }
}