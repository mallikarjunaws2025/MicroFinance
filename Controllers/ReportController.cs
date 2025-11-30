using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TestApp.Comman;
using TestApp.DB; 
using TestApp.VModels.Loans;
using TestApp.VModels.Member;
using TestApp.VModels.Reports;

namespace TestApp.Controllers
{
    public class ReportController : Controller
    {
        public ReportController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        string sGroupName = string.Empty;
        Int32 iNetMembers = 0;
        string sCrDt = string.Empty;
        string sExpiryDate = string.Empty;
        Double dLA = 0.00, dLP = 0.00;
        Double dBalanceInterest = 0.00;
        Double dTALRSavings = 0.00, dALRSavings = 0.00;
        Int32 iNoOfDay = 0, iTNoOfClts = 0;
        Double dPrinEMI = 0.00;
        Double dIntEMI = 0.00;
        Double dTotal = 0.00;
        Double dHTotal = 0.00, dTHLA = 0.00, dTHLPA = 0.00, dTHLI = 0.00, dTHALRColl,
         dHAdvanceALRColl = 0.00, dHAdvanceALRAdjusted = 0.00, dTotalHNetCash = 0.00, dTHPrin = 0.00, dTHInt = 0.00, dHTALRCollCur = 0.00;

        [HttpGet]
        public ActionResult DailyReport()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            MemberViewModel objMbr = new MemberViewModel();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                        select new SelectListItem
                                                        {
                                                            Text = p.GrpName,
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.GrpNameList = GrpCodeList;


                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffID.ToString()
                                                      }).ToList();
                    objMbr.StaffMbrList = StaffList;

                    return View(objMbr);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DailyReport() Get" + ex.InnerException);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoansReport()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            MemberViewModel objMbr = new MemberViewModel();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                        select new SelectListItem
                                                        {
                                                            Text = p.GrpName,
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.GrpNameList = GrpCodeList;


                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffID.ToString()
                                                      }).ToList();
                    objMbr.StaffMbrList = StaffList;

                    return View(objMbr);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DailyReport() Get" + ex.InnerException);
            }
            return View();
        }


        [HttpGet]
        public JsonResult GetLoansReportData(string sGrpName, string sStaffName)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false; 
            List<spLoansReport_Result> objListLoansReport = new List<spLoansReport_Result>();
            try
            {
                Decimal TAmount = 0;
                
                objListLoansReport = db.spLoansReport(sGrpName, sStaffName).OrderBy(x => x.StaffName).ThenBy(x => x.GrpName).ToList();

                if (objListLoansReport != null && objListLoansReport.Count > 0)
                {
                    // Calculate total amount
                    for (int i = 0; i < objListLoansReport.Count; i++)
                    { 
                        if (!string.IsNullOrEmpty(objListLoansReport[i].Loan_Amount))
                        {
                            TAmount += Convert.ToDecimal(objListLoansReport[i].Loan_Amount);
                        }
                    }

                    // Set row counts for hierarchical display
                    for (int i = 0; i < objListLoansReport.Count; i++)
                    {
                        objListLoansReport[i].GrpRowCount = objListLoansReport.Where(x => x.StaffName == objListLoansReport[i].StaffName && x.GrpName == objListLoansReport[i].GrpName).ToList().Count;
                        objListLoansReport[i].StaffRowCount = objListLoansReport.Where(x => x.StaffName == objListLoansReport[i].StaffName).ToList().Count; 
                    }

                    // Add total summary row
                    spLoansReport_Result obj = new spLoansReport_Result();
                    obj.MbrStatus = "End";
                    obj.Balance_Amount = Convert.ToString(TAmount);
                    objListLoansReport.Add(obj);
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetLoansReportData() Method: " + ex.Message, ex);
                return Json(new List<spLoansReport_Result>(), JsonRequestBehavior.AllowGet);
            }
            
            return Json(objListLoansReport, JsonRequestBehavior.AllowGet);
        }


             
        [HttpGet]
        public JsonResult GetDailyReportData(string sGrpName, string sStaffName)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            DailyReport objDailyReport = new VModels.Reports.DailyReport();
            List<DailyReport> objListDailyReport = new List<DailyReport>();

            List<spGetDailyDueReportsData_Result8> objData = new List<spGetDailyDueReportsData_Result8>();
            try
            {
                objDailyReport.SlNo = "0";

                if (sGrpName == "--All Groups--")
                {
                    sGrpName = null;
                }

                if (sStaffName == "--All Staff--")
                {
                    sStaffName = null;
                }

                if (!string.IsNullOrEmpty(sGrpName) && !string.IsNullOrEmpty(sStaffName))
                {
                    objData = db.spGetDailyDueReportsData(sGrpName, sStaffName).ToList();
                }
                else if (!string.IsNullOrEmpty(sGrpName) && string.IsNullOrEmpty(sStaffName))
                {
                    objData = db.spGetDailyDueReportsData(sGrpName, null).ToList();
                }
                else if (!string.IsNullOrEmpty(sStaffName) && string.IsNullOrEmpty(sGrpName))
                {
                    objData = db.spGetDailyDueReportsData(null, sStaffName).ToList();
                }
                else 
                {
                    objData = db.spGetDailyDueReportsData(sGrpName, sStaffName).ToList();
                }

                string sVal = string.Empty;
                if (objData.Count > 0)
                {
                    for (int i = 0; i <= objData.Count; i++)
                    {
                        sGroupName = objData[i].GrpName;

                        var gridData = (from r in objData
                                        where r.GrpName == sGroupName
                                        select r).ToArray();

                        for (int j = 0; j < gridData.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Net_Members)))
                                iNetMembers += Convert.ToInt32(gridData[j].Net_Members);

                            sCrDt = gridData[j].CrDt;

                            sExpiryDate = gridData[j].DOC;

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Loan_Amount)))
                                dLP += Convert.ToDouble(gridData[j].Loan_Amount);

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Loan_Amount)) && !string.IsNullOrEmpty(Convert.ToString(gridData[j].Balance_Interest)))
                                dLA += (Convert.ToDouble(gridData[j].Loan_Amount) + Convert.ToDouble(gridData[j].Balance_Interest));

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Balance_Interest)))
                                dBalanceInterest += Convert.ToDouble(gridData[j].Balance_Interest);

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].NetSavings)))
                                dTALRSavings += Convert.ToDouble(gridData[j].NetSavings);

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].ALRSavings)))
                                dALRSavings += Convert.ToDouble(gridData[j].ALRSavings);

                            iNoOfDay = Convert.ToInt32(gridData[j].NoOfDay);

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Prin_EMI)))
                                dPrinEMI += Convert.ToDouble(gridData[j].Prin_EMI);

                            if (!string.IsNullOrEmpty(Convert.ToString(gridData[j].Int_EMI)))
                                dIntEMI += Convert.ToDouble(gridData[j].Int_EMI);
                            dTotal = dPrinEMI + dIntEMI + dALRSavings;
                        }
                        iTNoOfClts += iNetMembers;
                        dTHLA += dLA;
                        dTHLI += dBalanceInterest;
                        dTHLPA += dLP;
                        dTHALRColl += dTALRSavings;
                        dTHPrin += dPrinEMI;
                        dTHInt += dIntEMI;
                        dHTALRCollCur += dALRSavings;
                        dHTotal += dTotal;
                        dHAdvanceALRColl += 0.00;
                        dHAdvanceALRAdjusted += 0.00;
                        dTotalHNetCash += dHTotal - (dHAdvanceALRColl + dHAdvanceALRAdjusted);

                        objDailyReport.SlNo = (Convert.ToInt32(objDailyReport.SlNo) + 1).ToString();
                        objDailyReport.GroupName = sGroupName;
                        objDailyReport.NoOfClts = iNetMembers.ToString();
                        objDailyReport.StartDt = sCrDt;
                        objDailyReport.EndDt = sExpiryDate;
                        objDailyReport.LA = dLA.ToString();
                        objDailyReport.LP = dLP.ToString();
                        objDailyReport.LI = dBalanceInterest.ToString();
                        objDailyReport.TALRCollected = dTALRSavings.ToString();
                        objDailyReport.Days = iNoOfDay.ToString();

                        objDailyReport.Principal = dPrinEMI.ToString();
                        objDailyReport.Interest = dIntEMI.ToString();
                        objDailyReport.ALRColl = dALRSavings.ToString();
                        objDailyReport.Total = dTotal.ToString();
                        objDailyReport.LastCount = i;
                        objListDailyReport.Add(objDailyReport);

                        DailyReport objHDailyReport = new VModels.Reports.DailyReport();
                        objHDailyReport.SlNo = "LastRecord";
                        objHDailyReport.iTNoOfClts = iTNoOfClts.ToString();
                        objHDailyReport.dTHLA = dTHLA.ToString();
                        objHDailyReport.dTHLI = dTHLI.ToString();
                        objHDailyReport.dTHLPA = dTHLPA.ToString();
                        objHDailyReport.dTHALRColl = dTHALRColl.ToString();
                        objHDailyReport.dTHPrin = dTHPrin.ToString();
                        objHDailyReport.dTHInt = dTHInt.ToString();
                        objHDailyReport.dHTALRCollCur = dHTALRCollCur.ToString();
                        objHDailyReport.dHTotal = dHTotal.ToString();
                        objHDailyReport.dHAdvanceALRColl = dHAdvanceALRColl.ToString();
                        objHDailyReport.dHAdvanceALRAdjusted = dHAdvanceALRAdjusted.ToString();
                        objHDailyReport.dTotalHNetCash = dTotalHNetCash.ToString();
                        objListDailyReport.Add(objHDailyReport);
                        return Json(objListDailyReport, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("No Data Found For criteria.", JsonRequestBehavior.AllowGet);
                }
               

            }
            catch (Exception ex)
            {
                logger.Error("Error in MemberList() Get Method" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        //*****************************************************************************************************************************
        [HttpGet]
        public ActionResult SummeryReport()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DailyReport() Get" + ex.InnerException);
            }
            return View();
        }

        


        [HttpGet]
        public JsonResult GetSummeyReportData(string sFrmDt, string sToDt)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<SummeryReport> objReportDataList = new List<SummeryReport>();
            
            try
            {
                if (!string.IsNullOrEmpty(sFrmDt) && !string.IsNullOrEmpty(sToDt))
                {
                    DateTime frmDate, toDate;
                    if (!DateTime.TryParse(sFrmDt, out frmDate) || !DateTime.TryParse(sToDt, out toDate))
                    {
                        return Json(new List<SummeryReport>(), JsonRequestBehavior.AllowGet);
                    }

                    int frmMonth = frmDate.Month;
                    int toMonth = toDate.Month;
                    int frmYear = frmDate.Year;
                    int toYear = toDate.Year;

                    // ==================== PROGRESS REPORT DATA ====================
                    int slNo = 1;

                    // 1. No Of Groups Formed
                    var allGroups = db.FinGroups.ToList();
                    
                    // BOR: Groups created BEFORE the from date that are still active
                    int borGroups = allGroups.Count(g => {
                        var crDt = ParseDateSafe(g.DOC);
                        if (crDt == null) return false;
                        // Created before from date
                        bool createdBefore = crDt.Value.Date < frmDate.Date;
                        // Still active: no DOC or DOC is after from date
                        var doc = ParseDateSafe(g.DOC);
                        bool stillActive = string.IsNullOrEmpty(g.DOC) || doc == null || doc.Value.Date >= frmDate.Date;
                        return createdBefore && stillActive;
                    });
                    
                    // Added: Groups created within the date range (from date to to date) and not cancelled
                    int addedGroups = allGroups.Count(g => {
                        var crDt = ParseDateSafe(g.DOC);
                        if (crDt == null) return false;
                        return crDt.Value.Date >= frmDate.Date && 
                               crDt.Value.Date <= toDate.Date &&
                               g.GStatus != "Cancel";
                    });
                    
                    // Dropped: Groups cancelled within the date range
                    int droppedGroups = allGroups.Count(g => {
                        var crDt = ParseDateSafe(g.DOC);
                        if (crDt == null) return false;
                        return crDt.Value.Date >= frmDate.Date && 
                               crDt.Value.Date <= toDate.Date &&
                               g.GStatus == "Cancel";
                    });
                    
                    int eorGroups = borGroups + addedGroups - droppedGroups;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "No Of Groups Formed", 
                        BOR = borGroups.ToString(), Added = addedGroups.ToString(), Dropped = droppedGroups.ToString(), EOR = eorGroups.ToString() });

                    // 2. No Of Active Clients
                    var allMembers = db.Members.ToList();
                    int borClients = allMembers.Count(m => ParseDateSafe(m.DOJ) != null && 
                        ParseDateSafe(m.DOJ).Value.Month < frmMonth && 
                        ParseDateSafe(m.DOJ).Value.Year == frmYear &&
                        m.MbrStatus == "Active");
                    
                    int addedClients = allMembers.Count(m => ParseDateSafe(m.DOJ) != null && 
                        ParseDateSafe(m.DOJ).Value.Month >= frmMonth && 
                        ParseDateSafe(m.DOJ).Value.Month <= toMonth &&
                        ParseDateSafe(m.DOJ).Value.Year == frmYear &&
                        m.MbrStatus == "Active");
                    
                    int droppedClients = allMembers.Count(m => ParseDateSafe(m.DOJ) != null && 
                        ParseDateSafe(m.DOJ).Value.Month >= frmMonth && 
                        ParseDateSafe(m.DOJ).Value.Month <= toMonth &&
                        ParseDateSafe(m.DOJ).Value.Year == frmYear &&
                        m.MbrStatus == "Cancel");
                    
                    int eorClients = borClients + addedClients - droppedClients;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "No Of Active Clients", 
                        BOR = borClients.ToString(), Added = addedClients.ToString(), Dropped = droppedClients.ToString(), EOR = eorClients.ToString() });

                    // 3. Current Loans
                    var allLoans = db.Loans.ToList();
                    // BOR: Loans disbursed before the period that are still active (not expired)
                    decimal borCurrentLoans = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month < frmMonth && 
                               disbDate.Value.Year == frmYear &&
                               string.IsNullOrEmpty(l.Expiry_Date);
                    }).Sum(l => ParseDecimalSafe(l.Loan_Amount));
                    
                    // Added: New loans disbursed within the period
                    decimal addedCurrentLoans = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month >= frmMonth && 
                               disbDate.Value.Month <= toMonth &&
                               disbDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Loan_Amount));
                    
                    // Dropped: Loans that EXPIRED/CLOSED within the period (based on Expiry_Date, not Date_Of_Disbursement)
                    decimal droppedCurrentLoans = allLoans.Where(l => {
                        var expDate = ParseDateSafe(l.Expiry_Date);
                        if (expDate == null) return false;
                        return expDate.Value.Month >= frmMonth && 
                               expDate.Value.Month <= toMonth &&
                               expDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Loan_Amount));
                    
                    decimal eorCurrentLoans = borCurrentLoans + addedCurrentLoans - droppedCurrentLoans;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Current Loans", 
                        BOR = borCurrentLoans.ToString("0.00"), Added = addedCurrentLoans.ToString("0.00"), 
                        Dropped = droppedCurrentLoans.ToString("0.00"), EOR = eorCurrentLoans.ToString("0.00") });

                    // 4. Loan Out Standing (using Balance_Amount which is the actual outstanding principal + Balance_Interest)
                    // BOR: Outstanding for loans disbursed before the period that are still active
                    decimal borOutstanding = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month < frmMonth && 
                               disbDate.Value.Year == frmYear &&
                               string.IsNullOrEmpty(l.Expiry_Date);
                    }).Sum(l => ParseDecimalSafe(l.Balance_Amount) + ParseDecimalSafe(l.Balance_Interest));
                    
                    // Added: Outstanding for loans disbursed within the period (new loans = full loan amount + interest)
                    decimal addedOutstanding = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month >= frmMonth && 
                               disbDate.Value.Month <= toMonth &&
                               disbDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Balance_Amount) + ParseDecimalSafe(l.Balance_Interest));
                    
                    // Dropped: Outstanding that was closed/expired within the period
                    decimal droppedOutstanding = allLoans.Where(l => {
                        var expDate = ParseDateSafe(l.Expiry_Date);
                        if (expDate == null) return false;
                        return expDate.Value.Month >= frmMonth && 
                               expDate.Value.Month <= toMonth &&
                               expDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Loan_Amount) + ParseDecimalSafe(l.Balance_Interest));
                    
                    decimal eorOutstanding = borOutstanding + addedOutstanding - droppedOutstanding;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Loan Out Standing", 
                        BOR = borOutstanding.ToString("0.00"), Added = addedOutstanding.ToString("0.00"), 
                        Dropped = droppedOutstanding.ToString("0.00"), EOR = eorOutstanding.ToString("0.00") });

                    // 5. Insurance Total Collected
                    decimal borInsurance = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month < frmMonth && 
                               disbDate.Value.Year == frmYear &&
                               string.IsNullOrEmpty(l.Expiry_Date);
                    }).Sum(l => (decimal)(l.Insurance ?? 0));
                    
                    decimal addedInsurance = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month >= frmMonth && 
                               disbDate.Value.Month <= toMonth &&
                               disbDate.Value.Year == frmYear;
                    }).Sum(l => (decimal)(l.Insurance ?? 0));
                    
                    decimal droppedInsurance = allLoans.Where(l => {
                        var expDate = ParseDateSafe(l.Expiry_Date);
                        if (expDate == null) return false;
                        return expDate.Value.Month >= frmMonth && 
                               expDate.Value.Month <= toMonth &&
                               expDate.Value.Year == frmYear;
                    }).Sum(l => (decimal)(l.Insurance ?? 0));
                    
                    decimal eorInsurance = borInsurance + addedInsurance - droppedInsurance;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Insurance Total Collected", 
                        BOR = borInsurance.ToString("0.00"), Added = addedInsurance.ToString("0.00"), 
                        Dropped = droppedInsurance.ToString("0.00"), EOR = eorInsurance.ToString("0.00") });

                    // 6. Total GRF Amt
                    decimal borGRF = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month < frmMonth && 
                               disbDate.Value.Year == frmYear &&
                               string.IsNullOrEmpty(l.Expiry_Date);
                    }).Sum(l => ParseDecimalSafe(l.GRFAmt));
                    
                    decimal addedGRF = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month >= frmMonth && 
                               disbDate.Value.Month <= toMonth &&
                               disbDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.GRFAmt));
                    
                    decimal droppedGRF = allLoans.Where(l => {
                        var expDate = ParseDateSafe(l.Expiry_Date);
                        if (expDate == null) return false;
                        return expDate.Value.Month >= frmMonth && 
                               expDate.Value.Month <= toMonth &&
                               expDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.GRFAmt));
                    
                    decimal eorGRF = borGRF + addedGRF - droppedGRF;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Total GRF Amt", 
                        BOR = borGRF.ToString("0.00"), Added = addedGRF.ToString("0.00"), 
                        Dropped = droppedGRF.ToString("0.00"), EOR = eorGRF.ToString("0.00") });

                    // 7. Total Passbook Fee (always 0)
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Total Passbook Fee", 
                        BOR = "0.00", Added = "0.00", Dropped = "0.00", EOR = "0.00" });

                    // 8. Total Loan Fee
                    decimal borLoanFee = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month < frmMonth && 
                               disbDate.Value.Year == frmYear &&
                               string.IsNullOrEmpty(l.Expiry_Date);
                    }).Sum(l => ParseDecimalSafe(l.Processing_Fee));
                    
                    decimal addedLoanFee = allLoans.Where(l => {
                        var disbDate = ParseDateSafe(l.Date_Of_Disbursement);
                        if (disbDate == null) return false;
                        return disbDate.Value.Month >= frmMonth && 
                               disbDate.Value.Month <= toMonth &&
                               disbDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Processing_Fee));
                    
                    decimal droppedLoanFee = allLoans.Where(l => {
                        var expDate = ParseDateSafe(l.Expiry_Date);
                        if (expDate == null) return false;
                        return expDate.Value.Month >= frmMonth && 
                               expDate.Value.Month <= toMonth &&
                               expDate.Value.Year == frmYear;
                    }).Sum(l => ParseDecimalSafe(l.Processing_Fee));
                    
                    decimal eorLoanFee = borLoanFee + addedLoanFee - droppedLoanFee;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = (slNo++).ToString(), Particulars = "Total Loan Fee", 
                        BOR = borLoanFee.ToString("0.00"), Added = addedLoanFee.ToString("0.00"), 
                        Dropped = droppedLoanFee.ToString("0.00"), EOR = eorLoanFee.ToString("0.00") });

                    // ==================== RECEIPTS AND PAYMENTS DATA ====================
                    var allLoanCols = db.Loan_Cols.ToList();
                    var allPartnerDetails = db.PartnerDetails.ToList();

                    // RECEIPTS
                    // 1. Opening Cash Balance (from PartnerDetails table)
                    decimal openingCashBalance = allPartnerDetails.Where(m => ParseDateSafe(m.CrDate) != null && 
                        ParseDateSafe(m.CrDate).Value.Month <= frmMonth &&
                        ParseDateSafe(m.CrDate).Value.Year == frmYear &&
                        ParseDateSafe(m.CrDate).Value.Month <= toMonth &&
                        ParseDateSafe(m.CrDate).Value.Year == toYear)
                        .Sum(m => ParseDecimalSafe(m.InvestmentAmt));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Opening Cash Balance", RRs = openingCashBalance.ToString("0.00") });

                    // 2. Loan Recovered
                    decimal loanRecovered = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.Collected_Amount));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Loan Recovered", RRs = loanRecovered.ToString("0.00") });

                    // 3. Interest Collected
                    decimal interestCollected = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.Int_Collect));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Interest Collected", RRs = interestCollected.ToString("0.00") });

                    // 4. ALR Collected
                    decimal alrCollected = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.ALRSavings));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "ALR Collected", RRs = alrCollected.ToString("0.00") });

                    // 5. Loan Fee
                    decimal loanFeeReceipt = allLoans.Where(l => ParseDateSafe(l.Date_Of_Disbursement) != null && 
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month >= frmMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == frmYear &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month <= toMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == toYear)
                        .Sum(l => ParseDecimalSafe(l.Processing_Fee));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Loan Fee", RRs = loanFeeReceipt.ToString("0.00") });

                    // 6. Group Registration Fee
                    decimal grfReceipt = allLoans.Where(l => ParseDateSafe(l.Date_Of_Disbursement) != null && 
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month >= frmMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == frmYear &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month <= toMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == toYear)
                        .Sum(l => ParseDecimalSafe(l.GRFAmt));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Group Registration Fee", RRs = grfReceipt.ToString("0.00") });

                    // 7. Passbook Fee
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Passbook Fee", RRs = "0.00" });

                    // 8. Insurance Premium (2.5% of Insurance)
                    decimal insurancePremium = allLoans.Where(l => ParseDateSafe(l.Date_Of_Disbursement) != null && 
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month >= frmMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == frmYear &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month <= toMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == toYear)
                        .Sum(l => (decimal)(l.Insurance ?? 0)) * 2.5m / 100m;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Insurance Premium", RRs = insurancePremium.ToString("0.00") });

                    // 9. Pre Principal
                    decimal prePrincipal = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.PrePrinAmt));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Pre Principal", RRs = prePrincipal.ToString("0.00") });

                    // 10. Pre Interest
                    decimal preInterest = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.PreInterestAmt));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Pre Interest", RRs = preInterest.ToString("0.00") });

                    // 11. BM Advance
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "BM Advance", RRs = "0.00" });

                    // 12. Total Receipts
                    decimal totalReceipts = openingCashBalance + loanRecovered + interestCollected + alrCollected + 
                        loanFeeReceipt + grfReceipt + insurancePremium + prePrincipal + preInterest;
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Receipts", Receipts = "Total Receipts", RRs = totalReceipts.ToString("0.00") });

                    // PAYMENTS
                    // 1. Loans Disbursed
                    decimal loansDisbursed = allLoans.Where(l => ParseDateSafe(l.Date_Of_Disbursement) != null && 
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month >= frmMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == frmYear &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Month <= toMonth &&
                        ParseDateSafe(l.Date_Of_Disbursement).Value.Year == toYear)
                        .Sum(l => ParseDecimalSafe(l.Loan_Amount));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Payments", Payments = "Loans Disbursed", PRs = loansDisbursed.ToString("0.00") });

                    // 2. ALR Adjustment
                    decimal alrAdjustment = allLoanCols.Where(lc => ParseDateSafe(lc.Transact_Date) != null && 
                        ParseDateSafe(lc.Transact_Date).Value.Month >= frmMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == frmYear &&
                        ParseDateSafe(lc.Transact_Date).Value.Month <= toMonth &&
                        ParseDateSafe(lc.Transact_Date).Value.Year == toYear)
                        .Sum(lc => ParseDecimalSafe(lc.Adjustment));
                    
                    objReportDataList.Add(new SummeryReport { SlNo = "Payments", Payments = "ALR Adjustment", PRs = alrAdjustment.ToString("0.00") });

                    // 3. Cash Closing Balance
                    decimal cashClosingBalance = totalReceipts - loansDisbursed - alrAdjustment;
                    objReportDataList.Add(new SummeryReport { SlNo = "Payments", Payments = "Cash Closing Balance", PRs = cashClosingBalance.ToString("0.00") });

                    // 4. Total Payments
                    decimal totalPayments = loansDisbursed + alrAdjustment + cashClosingBalance;
                    objReportDataList.Add(new SummeryReport { SlNo = "Payments", Payments = "Total Payments", PRs = totalPayments.ToString("0.00") });

                    return Json(objReportDataList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new List<SummeryReport>(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in Summery report() Get Method: " + ex.Message + " Inner: " + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return Json(new List<SummeryReport>(), JsonRequestBehavior.AllowGet);
            }
        }

        // Helper method to safely parse date strings
        private DateTime? ParseDateSafe(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;
            
            DateTime result;
            string[] formats = { 
                "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", 
                "dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm:ss", 
                "yyyy-MM-dd HH:mm:ss", "M/d/yyyy", "d/M/yyyy"
            };
            
            if (DateTime.TryParseExact(dateString.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return result;
            
            if (DateTime.TryParse(dateString.Trim(), out result))
                return result;
            
            return null;
        }

        // Helper method to safely parse decimal strings
        private decimal ParseDecimalSafe(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            
            decimal result;
            if (decimal.TryParse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            
            if (decimal.TryParse(value.Trim(), out result))
                return result;
            
            return 0;
        }

        [HttpGet]
        public ActionResult GetRecoveryReport()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            LoanCols objMbr = new LoanCols();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                        select new SelectListItem
                                                        {
                                                            Text = p.GrpName,
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.GrpCodeList = GrpCodeList;


                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffName
                                                      }).ToList();
                    objMbr.StaffMbrList = StaffList;

                    return View(objMbr);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in LoanCols() Get" + ex.InnerException);
                ModelState.AddModelError(string.Empty, "Error while saving member");
                return View();
            }
        }

        [HttpGet]
        public ActionResult DailyReport_New()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    // Create empty list for initial load
                    List<TestApp.VModels.Reports.DailyReport> reportData = new List<TestApp.VModels.Reports.DailyReport>();
                    
                    // Set ViewBag data for dropdowns
                    ViewBag.GrpNameList = (from p in db.FinGroups.AsEnumerable()
                                          select new SelectListItem
                                          {
                                              Text = p.GrpName,
                                              Value = p.GroupCode
                                          }).ToList();

                    ViewBag.StaffMbrList = (from p in db.Staffs.AsEnumerable()
                                           select new SelectListItem
                                           {
                                               Text = p.StaffName,
                                               Value = p.StaffID.ToString()
                                           }).ToList();

                    return View("DailyReport_New", reportData);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DailyReport_New() Get: " + ex.Message, ex);
            }
            return View("DailyReport_New", new List<TestApp.VModels.Reports.DailyReport>());
        }

        [HttpGet]
        public JsonResult GenerateDailyReport(string date, string groupId, string staffId)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            
            try
            {
                // Parse the date
                DateTime reportDate = DateTime.Now;
                if (!string.IsNullOrEmpty(date))
                {
                    DateTime.TryParse(date, out reportDate);
                }

                // Get loans with their group and member info
                var loansQuery = from l in db.Loans
                                 join m in db.Members on l.MbrId equals m.MbrId
                                 join g in db.FinGroups on l.GrpCode equals g.GroupCode
                                 select new
                                 {
                                     l.LoanID,
                                     l.MbrId,
                                     m.MbrName,
                                     g.GrpName,
                                     l.GrpCode,
                                     l.StaffName,
                                     l.Loan_Amount,
                                     l.Balance_Amount,
                                     l.NextDueDt
                                 };

                // Apply group filter
                if (!string.IsNullOrEmpty(groupId))
                {
                    loansQuery = loansQuery.Where(x => x.GrpCode == groupId);
                }

                // Apply staff filter
                if (!string.IsNullOrEmpty(staffId))
                {
                    int staffIdInt;
                    if (int.TryParse(staffId, out staffIdInt))
                    {
                        var staffName = db.Staffs.Where(s => s.StaffID == staffIdInt).Select(s => s.StaffName).FirstOrDefault();
                        if (!string.IsNullOrEmpty(staffName))
                        {
                            loansQuery = loansQuery.Where(x => x.StaffName == staffName);
                        }
                    }
                }

                var loans = loansQuery.ToList();

                // Build report data
                var reportData = new List<object>();
                int slNo = 1;
                decimal totalCollections = 0;
                decimal totalNetBalance = 0;
                decimal totalDisbursements = 0;
                decimal totalDue = 0;
                
                foreach (var loan in loans)
                {
                    // Get the latest Loan_Cols record for this loan to get PaidEMI and balance info
                    var latestLoanCol = db.Loan_Cols
                        .Where(lc => lc.LoanId == loan.LoanID)
                        .OrderByDescending(lc => lc.LoanColId)
                        .FirstOrDefault();

                    // Get all Loan_Cols for this loan to find today's collection
                    var allLoanCols = db.Loan_Cols
                        .Where(lc => lc.LoanId == loan.LoanID)
                        .ToList();

                    // Find today's collection by parsing Transact_Date and comparing with reportDate
                    decimal todayCollection = 0;
                    foreach (var lc in allLoanCols)
                    {
                        if (!string.IsNullOrEmpty(lc.Transact_Date))
                        {
                            DateTime transDate;
                            // Try general DateTime.TryParse first (handles many formats automatically)
                            if (DateTime.TryParse(lc.Transact_Date, out transDate))
                            {
                                if (transDate.Date == reportDate.Date)
                                {
                                    decimal collected = 0;
                                    decimal.TryParse(lc.Collected_Amount, out collected);
                                    todayCollection += collected;
                                }
                            }
                        }
                    }

                    decimal loanAmount = 0;
                    decimal.TryParse(loan.Loan_Amount?.ToString(), out loanAmount);
                    
                    decimal balanceAmount = 0;
                    decimal.TryParse(loan.Balance_Amount?.ToString(), out balanceAmount);
                    
                    // Get PaidEMI from the latest Loan_Cols record
                    int paidEMI = 0;
                    
                    // Get Principal Balance and Interest Balance from latest Loan_Cols
                    decimal prinBalance = 0;
                    decimal intBalance = 0;
                    
                    if (latestLoanCol != null)
                    {
                        int.TryParse(latestLoanCol.PaidEMI?.ToString(), out paidEMI);
                        decimal.TryParse(latestLoanCol.Prin_Balance, out prinBalance);
                        decimal.TryParse(latestLoanCol.Balance_Interest, out intBalance);
                    }
                    
                    // Net balance is principal balance + interest balance
                    decimal netBalance = prinBalance + intBalance;
                    if (netBalance == 0)
                    {
                        // Fallback to loan balance amount if Loan_Cols doesn't have the values
                        netBalance = balanceAmount;
                    }

                    totalCollections += todayCollection;
                    totalNetBalance += netBalance;
                    totalDisbursements += loanAmount;
                    totalDue += balanceAmount;

                    // Only add records that have either a collection today or have dues
                    if (todayCollection > 0 || balanceAmount > 0)
                    {
                        reportData.Add(new
                        {
                            SlNo = slNo.ToString(),
                            GroupName = loan.GrpName,
                            MemberName = loan.MbrName,
                            LoanID = loan.LoanID,
                            TodayCollection = todayCollection,
                            LoanAmount = loanAmount,
                            BalanceAmount = balanceAmount,
                            PaidEMI = paidEMI,
                            NetBalance = netBalance
                        });
                        slNo++;
                    }
                }

                return Json(new { 
                    success = true, 
                    data = reportData, 
                    count = reportData.Count,
                    totalCollections = totalCollections,
                    totalNetBalance = totalNetBalance,
                    totalDisbursements = totalDisbursements,
                    totalDue = totalDue
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error in GenerateDailyReport: " + ex.Message);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool CompareDates(string dbDateString, DateTime filterDate)
        {
            if (string.IsNullOrEmpty(dbDateString))
                return false;

            DateTime dbDate;
            string[] dateFormats = {
            "dd-MM-yyyy HH:mm:ss",
            "dd-MM-yyyy",
            "yyyy-MM-dd",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "M/d/yyyy"
        };

            if (DateTime.TryParseExact(dbDateString.Trim(), dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dbDate))
            {
                return dbDate.Date == filterDate.Date; // Compare only date parts, ignoring time
            }

            return false;
        }

        [HttpGet]
        public ActionResult ExportDailyReport(string date, string groupId, string reportType, string format)
        {
            try
            {
                // For now, redirect to the report page - export functionality can be enhanced later
                return RedirectToAction("DailyReport_New");
            }
            catch (Exception ex)
            {
                logger.Error("Error in ExportDailyReport: " + ex.Message);
                return RedirectToAction("DailyReport_New");
            }
        }
    }
}
