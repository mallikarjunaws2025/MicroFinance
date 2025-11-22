using System;

// Legacy type aliases for compatibility with existing code
// This allows old TestApp.DB references to work with our new Model First entities

namespace TestApp.DB
{
    // Entity type aliases (redirecting to TestApp.Models)
    using Staff = TestApp.Models.Staff;
    using Member = TestApp.Models.Member;
    using Branch = TestApp.Models.Branch;
    using Loan = TestApp.Models.Loan;
    using Loan_Cols = TestApp.Models.Loan_Cols;
    using ALRAudjustment = TestApp.Models.ALRAudjustment;
    using FinGroup = TestApp.Models.FinGroup;
    using Management = TestApp.Models.Management;
    using PartnerDetail = TestApp.Models.PartnerDetail;
    using StaffLogin = TestApp.Models.StaffLogin;
    using TmpReceipts = TestApp.Models.TmpReceipts;
    using TmpSummaryReportData = TestApp.Models.TmpSummaryReportData;
    using TmpLoanColsBackUpData = TestApp.Models.TmpLoanColsBackUpData;
    using TmpLoansBackUpData = TestApp.Models.TmpLoansBackUpData;
    using UserLog = TestApp.Models.UserLog;
    
    // Context type alias (redirecting to TestApp.Data)
    using MicroFinanceEntities = TestApp.Data.MicroFinanceContext;
    
    // Legacy class declarations for backward compatibility
    public class StaffLogin : TestApp.Models.StaffLogin { }
}