using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    // Stored procedure result classes for compatibility
    
    public class spLoansReport_Result
    {
        public string LoanId { get; set; }
        public string MbrName { get; set; }
        public string GroupName { get; set; }
        public decimal? LoanAmount { get; set; }
        public DateTime? LoanDate { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Status { get; set; }
    }

    public class spGetDailyDueReportsData_Result8
    {
        public string MbrName { get; set; }
        public string GroupName { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? CollectedAmount { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
    }

    // Type aliases for compatibility with existing naming
    public class TmpSummeryReportData : TmpSummaryReportData { }
    public class TmpReceipt : TmpReceipts { }
}