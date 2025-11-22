using TestApp.DB; 
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TestApp.Comman;
using TestApp.VModels.Member;
using TestApp.VModels.Reports;
using TestApp.VModels.Loans;

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
            SummeryReport objSummeryReport = null;
            try
            {
                if (!string.IsNullOrEmpty(sFrmDt) && !string.IsNullOrEmpty(sToDt))
                {

                    db.spGetProgressReportData(sFrmDt, sToDt);

                    foreach (TmpSummeryReportData obj in db.TmpSummeryReportDatas.ToList())
                    {
                        objSummeryReport = new SummeryReport();
                        objSummeryReport.SlNo = obj.SlNo.ToString();
                        objSummeryReport.Particulars = obj.Particulars;
                        objSummeryReport.BOR = obj.BOR;
                        objSummeryReport.Added = obj.Added;
                        objSummeryReport.Dropped = obj.Dropped;
                        objSummeryReport.EOR = obj.EOR;

                        objReportDataList.Add(objSummeryReport);
                    }


                    db.spGetReceiptsAndPaymentsData(sFrmDt, sToDt);
                    bool bIsPayments = false;
                    foreach (TmpReceipt obj in db.TmpReceipts.ToList())
                    {
                        objSummeryReport = new SummeryReport();
                        if (!String.IsNullOrEmpty(Convert.ToString(obj.Receipts)) && Convert.ToString(obj.Receipts) != "Loans Disbursed" &&
                            !bIsPayments)
                        {
                            objSummeryReport.SlNo = "Receipts";
                            objSummeryReport.Receipts = obj.Receipts;
                            objSummeryReport.RRs = obj.Rs;

                            objReportDataList.Add(objSummeryReport);
                        }
                        else 
                        {
                            bIsPayments = true;
                            objSummeryReport.SlNo = "Payments";
                            objSummeryReport.Payments = obj.Receipts;
                            objSummeryReport.PRs = obj.Rs;

                            objReportDataList.Add(objSummeryReport);
                        }
                        
                    }

                    return Json(objReportDataList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in Summery report() Get Method" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
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
    }
}
