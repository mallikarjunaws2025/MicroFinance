using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TestApp.Comman;
using TestApp.DB;
using TestApp.VModels.Loans;
using TestApp.VModels.Member;

namespace TestApp.Controllers
{
    public class LoansController : Controller
    {

        public LoansController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
            ViewBag.ImagePath = @"~/Resource/";
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        string sLListGrpCode = string.Empty;
        [HttpGet]
        public ActionResult LoansDisbus(int iMbrID)
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    LoanDisbus objLoanDisbus = new LoanDisbus();

                    List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                        select new SelectListItem
                                                        {
                                                            Text = p.GrpName,
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objLoanDisbus.GrpCodeList = GrpCodeList;


                    List<SelectListItem> MbrList = (from p in db.Members.AsEnumerable()
                                                    select new SelectListItem
                                                    {
                                                        Text = p.MbrName,
                                                        Value = p.MbrId.ToString()
                                                    }).ToList();
                    objLoanDisbus.MbrList = MbrList;




                    //if (!string.IsNullOrEmpty(Convert.ToString(Session["LoanLaterDisbusMbrID"])) ||
                    //    !string.IsNullOrEmpty(Convert.ToString(Session["SavedMbrID"])))
                    if (iMbrID > 0)
                    {

                        objLoanDisbus.IsSucess = "NewMbr";

                        if (TempData["GType"] != null)
                        {
                            objLoanDisbus.GType = Convert.ToString(TempData["GType"]);
                        }
                        else if (Session["GType"] != null)
                        {
                            objLoanDisbus.GType = Convert.ToString(Session["GType"]);
                        }
                    }
                    else
                    {
                        objLoanDisbus.IsSucess = "Reneval";
                    }
                    return View(objLoanDisbus);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansDisbus () Get" + ex.InnerException);
                
                // Return a default model even in case of error
                LoanDisbus defaultModel = new LoanDisbus();
                defaultModel.GrpCodeList = new List<SelectListItem>();
                defaultModel.MbrList = new List<SelectListItem>();
                return View(defaultModel);
            }
            
            // This should never be reached, but just in case
            return View(new LoanDisbus() 
            { 
                GrpCodeList = new List<SelectListItem>(),
                MbrList = new List<SelectListItem>()
            });

        }

        [HttpPost]
        public ActionResult LoansDisbus(LoanDisbus objLoans)
        {
            LoanDisbus objLoanDisbus = new LoanDisbus();
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            Loan objDBLoans = new Loan();
            Member objCrMbr = new Member();
            Loan_Cols dbLoanCols = new Loan_Cols();
            FinGroup objGrp = new FinGroup();
            DateTime NxtDueDt;
            int iStaffID = 0, iMbrID = 0;
            double dIntEMIAmt = 0.00;
            string sStaffName = string.Empty, sExistingLoanType = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SavedMbrID"])) || !string.IsNullOrEmpty(Convert.ToString(objLoans.MbrId)))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString((Session["SavedMbrID"]))))
                    {
                        iMbrID = Convert.ToInt32(Session["SavedMbrID"]);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(objLoans.MbrId)))
                    {
                        iMbrID = Convert.ToInt32(objLoans.MbrId);
                    }

                    objDBLoans = db.Loans.Where(p => p.MbrId == objLoans.MbrId).FirstOrDefault();
                    if (objDBLoans != null)
                    {
                        if (!string.IsNullOrEmpty(objDBLoans.Expiry_Date))
                        {
                            DateTime exdt = Convert.ToDateTime(objDBLoans.Expiry_Date);
                            if (exdt > DateTime.Now)
                            {
                                objLoanDisbus.IsSucess = "L";
                            }
                        }

                        if (!string.IsNullOrEmpty(objDBLoans.Balance_Amount))
                        {
                            if (Convert.ToInt32(objDBLoans.Balance_Amount) > 10)
                            {
                                objLoanDisbus.IsSucess = "L";
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(objLoanDisbus.IsSucess))
                {
                    objCrMbr = db.Members
                               .Where(p => p.MbrId == iMbrID)
                               .Select(p => p).FirstOrDefault();


                    if (objCrMbr != null)
                    {

                        objGrp = (from c in db.FinGroups
                                  where c.GroupCode == objCrMbr.GroupCode
                                  select c).FirstOrDefault();

                        iStaffID = Convert.ToInt32(objCrMbr.StaffId);
                    }

                    // IntrAmount/NoOfWeeks
                    // IntrRate%NoOfWeeks

                    //if(!string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["IntrRate"])));

                    //dIntEMIAmt = (((Convert.ToDouble(objLoans.Loan_Amount) * Convert.ToDouble(objLoans.RateOfInterest)) / 100) / Convert.ToInt32(objLoans.NoOfDays));
                    dIntEMIAmt = (Convert.ToDouble(objLoans.RateOfInterest) / Convert.ToInt32(objLoans.NoOfDays));
                    objDBLoans = objDBLoans == null ? new Loan() : objDBLoans;
                    objDBLoans.Int_EMI = dIntEMIAmt.ToString();

                    //objLoans.Date_Of_Disbursement = objLoans.Date_Of_Disbursement.ToString("MM/dd/yyyy");
                    NxtDueDt = DateTime.Parse(objLoans.Date_Of_Disbursement, new CultureInfo("en-US", true));

                    if (objGrp.GType.Trim() == "Daily")
                    {
                        NxtDueDt = NxtDueDt.AddDays(1);
                    }
                    else
                    {


                        NxtDueDt = Convert.ToDateTime(objLoans.NextDueDt); // NxtDueDt.AddDays(7 - (int)NxtDueDt.DayOfWeek);

                    }


                    objGrp.DOC = objLoans.Expiry_Date;
                    db.Entry(objGrp).State = EntityState.Modified;
                    db.SaveChanges();
                    #region Loan insert

                    sStaffName = db.Staffs
                              .Where(p => p.StaffID == iStaffID)
                              .Select(p => p.StaffName).FirstOrDefault();

                    objDBLoans.MbrId = objCrMbr.MbrId;
                    objDBLoans.Loan_Amount = objLoans.Loan_Amount == null ? "0" : objLoans.Loan_Amount;

                    objDBLoans.Balance_Amount = objLoans.Loan_Amount == null ? "0" : objLoans.Loan_Amount;
                    objDBLoans.Prin_EMI = objLoans.Prin_EMI == null ? "0" : objLoans.Prin_EMI;
                    objDBLoans.Int_EMI = objDBLoans.Int_EMI == null ? "0" : objDBLoans.Int_EMI;
                    objDBLoans.NoOfDay = objLoans.NoOfDays == null ? 0 : objLoans.NoOfDays;
                    objDBLoans.Date_Of_Disbursement = objLoans.Date_Of_Disbursement;// objLoans.CrM + "/" + objLoans.CrD + "/" + objLoans.CrY;
                    objDBLoans.Expiry_Date = objLoans.Expiry_Date;
                    objDBLoans.Processing_Fee = objLoans.Processing_Fee == null ? "0" : objLoans.Processing_Fee;
                    objDBLoans.Stationary = objLoans.Stationary == null ? 0 : objLoans.Stationary;
                    objDBLoans.Insurance = objLoans.Insurance == null ? 0 : objLoans.Insurance;
                    objDBLoans.Other_Income = objLoans.Other_Income == null ? "0" : objLoans.Other_Income;
                    objDBLoans.NetSavings = objLoans.Savings == null ? "0" : objLoans.Savings;
                    objDBLoans.ALRSavings = objLoans.ALRAmt == null ? "0" : objLoans.ALRAmt;
                    objDBLoans.GRFAmt = objLoans.GRFAmt == null ? "0" : objLoans.GRFAmt;
                    objDBLoans.RateOfInterest = objLoans.RateOfInterest;
                    objDBLoans.GrpCode = objGrp.GroupCode == null ? "0" : objGrp.GroupCode;
                    objDBLoans.StaffName = sStaffName;
                    objDBLoans.Balance_Interest = Math.Round(Convert.ToDouble(objDBLoans.Int_EMI) * Convert.ToInt32(objDBLoans.NoOfDay)).ToString();
                    objDBLoans.Balance_Interest = objDBLoans.Balance_Interest == null ? "0" : objDBLoans.Balance_Interest;
                    objDBLoans.NextDueDt = NxtDueDt.ToShortDateString();
                    objDBLoans.LoanType = objLoans.LoanType;
                    objDBLoans.Status = 0;
                    db.Loans.Add(objDBLoans);
                    db.SaveChanges();

                    objCrMbr.MbrStatus = "Active";
                    db.Entry(objCrMbr).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region LoanCols Insert


                    double Actual_Balance = 0.00, dPrinceAmt = 0.00, dIntrestAmt = 0.00, dPrinDue = 0.00, dIntDue = 0.00;

                    if (!string.IsNullOrEmpty(objDBLoans.Loan_Amount) && !string.IsNullOrEmpty(Convert.ToString(objDBLoans.NoOfDay)))
                    {
                        dPrinceAmt = Convert.ToDouble(objDBLoans.Loan_Amount);

                        dIntrestAmt = Math.Round(Convert.ToDouble(objDBLoans.Int_EMI) * Convert.ToInt32(objDBLoans.NoOfDay));

                        Actual_Balance = Math.Round(dPrinceAmt + dIntrestAmt);

                        dPrinDue = Convert.ToDouble(objDBLoans.Prin_EMI);
                        dIntDue = Convert.ToDouble(objDBLoans.Int_EMI);
                    }


                    dbLoanCols.LoanId = objDBLoans.LoanID;
                    dbLoanCols.MbrId = objDBLoans.MbrId;
                    dbLoanCols.Collected_Amount = "0.00";
                    dbLoanCols.Actual_Balance = Convert.ToString(Actual_Balance) == null ? "0" : Convert.ToString(Actual_Balance);
                    dbLoanCols.Prin_Balance = objDBLoans.Loan_Amount == null ? "0" : objDBLoans.Loan_Amount;
                    dbLoanCols.Int_Collect = "0.00";
                    dbLoanCols.Prin_Collected = "0.00";
                    dbLoanCols.Balance_Installment = Convert.ToInt32(objDBLoans.NoOfDay) == null ? 0 : Convert.ToInt32(objDBLoans.NoOfDay);
                    dbLoanCols.Balance_Interest = Convert.ToString(dIntrestAmt) == null ? "0" : Convert.ToString(dIntrestAmt);
                    dbLoanCols.Prin_Due = Convert.ToString(dPrinDue) == null ? "0" : Convert.ToString(dPrinDue);
                    dbLoanCols.Int_Due = Convert.ToString(dIntDue) == null ? "0" : Convert.ToString(dIntDue);
                    dbLoanCols.Transact_Date = objDBLoans.Date_Of_Disbursement;
                    dbLoanCols.Next_Due_Date = Convert.ToDateTime(objLoans.Date_Of_Disbursement).AddDays(7).ToShortDateString();
                    dbLoanCols.Upto_Last_Savings = "0.00";
                    dbLoanCols.ALRSavings = Convert.ToString(objLoans.Savings) == null ? "0" : Convert.ToString(objLoans.Savings);
                    dbLoanCols.As_On = Convert.ToString(objLoans.Savings) == null ? "0" : Convert.ToString(objLoans.Savings);
                    dbLoanCols.Adjustment = "0";
                    dbLoanCols.PaidEMI = "0";

                    db.Loan_Cols.Add(dbLoanCols);
                    db.SaveChanges();
                    objLoanDisbus.IsSucess = "1";
                    objLoanDisbus.Savings = objCrMbr.MbrName;

                    //int iAdvancedEMI = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AdvancedEMI"]);
                    // If Advanced EMI Paid
                    if (objLoans.AdvancedEMI != null && Convert.ToDouble(objLoans.AdvancedEMI) > 0)
                    {

                        int iPaidEMI = Convert.ToInt32(objLoans.AdvancedEMI);  //string.IsNullOrEmpty(objLoans.AdvancedEMI) ? iAdvancedEMI :
                        for (int i = 1; i <= iPaidEMI; i++)
                        {

                            Loan_Cols dbAdvPaidLoanCols = new Loan_Cols();
                            Loan objAdvLoan = new Loan();

                            objAdvLoan = db.Loans
                             .Where(p => p.LoanID == dbLoanCols.LoanId)
                           .OrderByDescending(p => p.LoanID)
                           .Select(p => p).FirstOrDefault();

                            dbAdvPaidLoanCols = db.Loan_Cols
                              .Where(p => p.LoanId == dbLoanCols.LoanId)
                            .OrderByDescending(p => p.LoanColId)
                            .Select(p => p).FirstOrDefault();

                            if (objGrp.GType.Trim() == "Daily" && i > 1)
                            {
                                NxtDueDt = Convert.ToDateTime(dbLoanCols.Next_Due_Date).AddDays(1);
                            }
                            else
                            {
                                NxtDueDt = Convert.ToDateTime(dbLoanCols.Next_Due_Date).AddDays(7);
                            }

                            dbAdvPaidLoanCols.PostedUserID = Convert.ToString(Session["UserID"]);


                            dbAdvPaidLoanCols.LoanId = dbLoanCols.LoanId;
                            dbAdvPaidLoanCols.MbrId = dbLoanCols.MbrId;
                            dbAdvPaidLoanCols.Collected_Amount = Convert.ToString(Math.Round(Convert.ToDouble(dbAdvPaidLoanCols.Int_Due) + Convert.ToDouble(dbAdvPaidLoanCols.Prin_Due)));
                            dbAdvPaidLoanCols.Actual_Balance = Convert.ToString(Math.Round(Convert.ToDouble(dbAdvPaidLoanCols.Actual_Balance) - Convert.ToDouble(Math.Round(Convert.ToDouble(dbAdvPaidLoanCols.Int_Due) + Convert.ToDouble(dbAdvPaidLoanCols.Prin_Due)))));
                            dbAdvPaidLoanCols.Prin_Balance = Convert.ToString(Math.Round(Convert.ToDouble(dbAdvPaidLoanCols.Prin_Balance) - Convert.ToDouble(dbAdvPaidLoanCols.Prin_Due)));
                            dbAdvPaidLoanCols.Int_Collect = Convert.ToString(Convert.ToDouble(objAdvLoan.Int_EMI));
                            dbAdvPaidLoanCols.Prin_Collected = Convert.ToString(Convert.ToDouble(objAdvLoan.Prin_EMI));
                            dbAdvPaidLoanCols.Balance_Interest = Convert.ToString(Math.Round(Convert.ToDouble(objAdvLoan.Balance_Interest) - Convert.ToDouble(objAdvLoan.Int_EMI)));
                            dbAdvPaidLoanCols.Prin_Due = Convert.ToString(Convert.ToDouble(objAdvLoan.Prin_EMI));
                            dbAdvPaidLoanCols.Int_Due = Convert.ToString(Convert.ToDouble(objAdvLoan.Int_EMI));
                            dbAdvPaidLoanCols.Transact_Date = Convert.ToString(DateTime.Now.ToShortDateString());
                            dbAdvPaidLoanCols.Next_Due_Date = NxtDueDt.ToShortDateString();
                            dbAdvPaidLoanCols.Upto_Last_Savings = dbAdvPaidLoanCols.Upto_Last_Savings;
                            dbAdvPaidLoanCols.ALRSavings = Convert.ToString(Math.Round(Convert.ToDouble(dbAdvPaidLoanCols.ALRSavings)));
                            dbAdvPaidLoanCols.As_On = dbAdvPaidLoanCols.As_On;
                            dbAdvPaidLoanCols.Adjustment = dbAdvPaidLoanCols.Adjustment;
                            dbAdvPaidLoanCols.PaidEMI = Convert.ToString(Convert.ToInt32(dbAdvPaidLoanCols.PaidEMI) + 1);
                            dbAdvPaidLoanCols.Balance_Installment = Convert.ToInt32(dbAdvPaidLoanCols.Balance_Installment) - 1;
                            dbAdvPaidLoanCols.PostedUserID = Convert.ToString(Session["UserID"]);

                            objAdvLoan.Balance_Amount = Convert.ToString(Math.Round(Convert.ToDouble(objAdvLoan.Balance_Amount) - Convert.ToDouble(objAdvLoan.Prin_EMI)));
                            objAdvLoan.NextDueDt = NxtDueDt.ToShortDateString();
                            objAdvLoan.Balance_Interest = Convert.ToString(Math.Round(Convert.ToDouble(objAdvLoan.Balance_Interest) - Convert.ToDouble(Convert.ToDouble(objAdvLoan.Int_EMI)))); ;

                            db.Loan_Cols.Add(dbAdvPaidLoanCols);
                            db.SaveChanges();

                            db.Entry(objAdvLoan).State = EntityState.Modified;
                            db.SaveChanges();

                            objLoanDisbus.IsSucess = "1";
                            //}
                        }
                    }
                    Session["SavedMbrID"] = null;
                    #endregion
                }
                #region UI binding
                List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                    select new SelectListItem
                                                    {
                                                        Text = p.GrpName,
                                                        Value = p.GroupCode
                                                    }).ToList();
                objLoanDisbus.GrpCodeList = GrpCodeList;


                List<SelectListItem> MbrList = (from p in db.Members.AsEnumerable()
                                                select new SelectListItem
                                                {
                                                    Text = p.MbrName,
                                                    Value = p.MbrId.ToString()
                                                }).ToList();
                objLoanDisbus.MbrList = MbrList;
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansDisbus () Post" + ex.InnerException);
                objLoanDisbus.IsSucess = "2";
            }
            return View(objLoanDisbus);
        }

        [HttpGet]
        public ActionResult LoansCols()
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
        public string ValidateMember(int iMbrID)
        {
            string sMsg = string.Empty;
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(iMbrID)))
                {
                    Member objMbr = new Member();

                    var Data = db.Members
                                 .Where(p => p.MbrId == iMbrID)
                                 .Select(p => p).FirstOrDefault();

                    int igrpId = Convert.ToInt32(Data.GroupCode);

                    var gData = db.FinGroups
                                 .Where(p => p.GroupID == igrpId)
                                 .Select(p => p).FirstOrDefault();

                    sMsg = gData.GroupCode + "/" + Data.MbrName + "/" + Data.MbrStatus + "/" + Data.MbrAddress + "/" + Data.Husbandname;

                    logger.Info(Session["UserID"] + ": Loan reneval process " + Data.MbrName + DateTime.Now);
                    return sMsg;
                }
                else
                {
                    return sMsg;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in validateMember () Get" + ex.InnerException);
                return sMsg;
            }
        }

        [HttpGet]
        public JsonResult LoansList(string sGrpName, string sStaffName, string sMbrID, string DueDt)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            int iMbrID = 0;
            try
            {
                if (string.IsNullOrEmpty(sGrpName) || sGrpName.Trim() == "--All Groups--")
                {
                    sGrpName = null;
                }

                if (string.IsNullOrEmpty(sStaffName) || sStaffName.Trim() == "--All Staff--")
                {
                    sStaffName = null;
                }
                if (string.IsNullOrEmpty(sMbrID))
                {
                    sMbrID = null;
                }


                iMbrID = Convert.ToInt32(sMbrID);
                TempData["LoansPostingData"] = sGrpName + "," + sStaffName + "," + Convert.ToInt32(sMbrID);
                TempData.Keep("LoansPostingData");


                IEnumerable<spGetLoanMbrsList_Result6> RawGetLoanMbrsList = db.spGetLoanMbrsList(null, null, 0);



                if (!string.IsNullOrEmpty(sGrpName))
                {
                    RawGetLoanMbrsList = RawGetLoanMbrsList.Where(x => x.GrpName == sGrpName);
                }

                if (!string.IsNullOrEmpty(sStaffName))
                {
                    RawGetLoanMbrsList = RawGetLoanMbrsList.Where(x => x.StaffName == sStaffName);
                }

                if (iMbrID > 0)
                    RawGetLoanMbrsList = RawGetLoanMbrsList.Where(x => x.MbrId == iMbrID);



                if (!string.IsNullOrEmpty(DueDt))
                {
                    DateTime dt = Convert.ToDateTime(DueDt);

                    DueDt = dt.Month.ToString() + "/" + dt.Day.ToString() + "/" + dt.Year.ToString();

                    RawGetLoanMbrsList = RawGetLoanMbrsList.Where(x => x.Next_Due_Date == DueDt);
                }

                return Json(RawGetLoanMbrsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansList () Get" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }




        [HttpGet]
        public bool GetMember(int iMbrID)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(iMbrID)))
                {
                    Session["MbrID"] = iMbrID;
                    Session["IsEditableMemberInfo"] = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetMember() Get" + ex.InnerException);
                return false;
            }
        }


        [HttpGet]
        public JsonResult RemoveMember(int iMbrID)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(iMbrID)))
                {
                    Member objMbr = new Member();

                    var data = from c in db.Members
                               where c.MbrId == iMbrID
                               select c;

                    foreach (var sval in data.ToArray())
                    {
                        objMbr = sval;
                    }

                    db.Members.Remove(objMbr);
                    db.SaveChanges();
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in RemoveMember() Get" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult EditMember()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                MemberViewModel objMbr = new MemberViewModel();
                if (!string.IsNullOrEmpty(Convert.ToString(Session["IsEditableMemberInfo"])) &&
                        Convert.ToBoolean(Session["IsEditableMemberInfo"]) == true)
                {
                    #region Edit Member Info Black

                    MicroFinanceEntities db = new MicroFinanceEntities();

                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffID.ToString()
                                                      }).ToList();
                    List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                        select new SelectListItem
                                                        {
                                                            Text = p.GrpName,
                                                            Value = p.GroupCode.ToString()
                                                        }).ToList();
                    objMbr.StaffMbrList = StaffList;
                    objMbr.GrpNameList = GrpCodeList;
                    #endregion

                    int iMbrID = 0;
                    Member MbrObj = new Member();
                    FinGroup GrpID = new FinGroup();
                    Dictionary<int, string> myDic = new Dictionary<int, string>();

                    if (!string.IsNullOrEmpty(Convert.ToString(Session["MbrID"])))
                    {
                        iMbrID = Convert.ToInt32(Session["MbrID"]);

                        MbrObj = db.Members
                               .Where(p => p.MbrId == iMbrID)
                               .Select(p => p).FirstOrDefault();

                        GrpID = db.FinGroups
                              .Where(p => p.GroupCode == MbrObj.GroupCode)
                              .Select(p => p).FirstOrDefault();

                        objMbr.MbrName = MbrObj.MbrName;
                        objMbr.GrpCode = GrpID.GroupID.ToString();
                        objMbr.HsbndName = MbrObj.Husbandname;
                        objMbr.Age = (int)MbrObj.Age;
                        objMbr.MbrStatus = MbrObj.MbrStatus;
                        objMbr.StaffID = (int)MbrObj.StaffId;
                        objMbr.MbrAddress = MbrObj.MbrAddress;
                        objMbr.Gen = objMbr.Gen;
                        objMbr.CantactNum = objMbr.CantactNum;
                        objMbr.MbrDOJ = Convert.ToString(objMbr.CrD + "/" + objMbr.CrM + "/" + objMbr.CrY);
                        DateTime dt = Convert.ToDateTime(MbrObj.WithdrawDt);
                        objMbr.WD = dt.Day.ToString();
                        objMbr.WM = dt.Month.ToString();
                        objMbr.WD = Convert.ToString(dt.Day);
                        objMbr.WY = dt.Year.ToString();

                    }

                    objMbr.IsSucess = "0";
                    return View(objMbr);
                }
                return View(objMbr);
            }
            else
            {
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        [HttpPost]
        public ActionResult EditMember(MemberViewModel objMbr)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["IsEditableMemberInfo"])) &&
                                                      Convert.ToBoolean(Session["IsEditableMemberInfo"]) == true)
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(Session["MbrID"])))
                    {
                        MicroFinanceEntities db = new MicroFinanceEntities();
                        FinGroup Data = new FinGroup();
                        int iGrpID = Convert.ToInt32(objMbr.GrpCode);
                        Data = db.FinGroups
                                         .Where(p => p.GroupID == iGrpID)
                                         .Select(p => p).FirstOrDefault();

                        Member objCrMbr = new Member();
                        objCrMbr.MbrName = objMbr.MbrName;
                        objCrMbr.GroupCode = Data.GroupCode;
                        objCrMbr.Husbandname = objMbr.HsbndName;
                        objCrMbr.Age = objMbr.Age;
                        objCrMbr.MbrStatus = objMbr.MbrStatus;
                        objCrMbr.Gender = objMbr.Gen;
                        objCrMbr.CantactNo = objMbr.CantactNum;
                        objCrMbr.DOJ = Convert.ToString(objMbr.CrD + "/" + objMbr.CrM + "/" + objMbr.CrY);
                        objCrMbr.WithdrawDt = Convert.ToString(DateTime.Now); // Convert.ToDateTime(objMbr.CrD + "/" + objMbr.CrM + "/" + objMbr.CrY);
                        objCrMbr.StaffId = objMbr.StaffID;
                        objCrMbr.MbrAddress = objMbr.MbrAddress;


                        objCrMbr.MbrId = Convert.ToInt32(Session["MbrID"]);
                        db.Entry(objCrMbr).State = EntityState.Modified;
                        db.SaveChanges();
                        objMbr.IsSucess = "1";
                        logger.Info(Session["UserID"] + ": Edited Member details of " + objMbr.MbrName + DateTime.Now);
                        List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                          select new SelectListItem
                                                          {
                                                              Text = p.StaffName,
                                                              Value = p.StaffID.ToString()
                                                          }).ToList();
                        List<SelectListItem> GrpCodeList = (from p in db.FinGroups.AsEnumerable()
                                                            select new SelectListItem
                                                            {
                                                                Text = p.GrpName,
                                                                Value = p.GroupCode.ToString()
                                                            }).ToList();
                        objMbr.StaffMbrList = StaffList;
                        objMbr.GrpNameList = GrpCodeList;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in Edit Member () Post" + ex.InnerException);
                objMbr.IsSucess = "2";
            }
            return View(objMbr);
        }


        [HttpPost]
        public ActionResult LoansCols(LoanCols objLoanCols)
        {

            try
            {
                if (objLoanCols.PrepaidLoanIds != null)
                    Session["LoansIDs"] = objLoanCols.PrepaidLoanIds;
                //return RedirectToAction("LoanPosting", "Loans", objLoanCols);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("LoanPosting", "Loans");
        }

        [HttpGet]
        public string LoanPosting(string sPostType, string LoanId, string LoanAmount, string PaidInstallments, string PaidAmount, string BalanceAmount, string CurrentDue, string Savings, string sAdjustment)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            Loan_Cols objLoanColsdb = new Loan_Cols();
            Loan objLoan = new Loan();
            LoanCols objLoanCols = new LoanCols();
            FinGroup objFinGroup = new FinGroup();
            Member objMbr = new Member();
            string sGType = string.Empty, sIsSucess = string.Empty;
            TmpLoansBackUpData objTmpLoansBackUpData = new TmpLoansBackUpData();
            int iLoanID = 0;
            var PostingLoans = new List<spGetLoanMbrsList_Result>();
            try
            {
                iLoanID = Convert.ToInt32(LoanId);

                #region Loan update

                objLoanCols.LoanId = iLoanID;

                objLoan = db.Loans
                                .Where(p => p.LoanID == objLoanCols.LoanId)
                                .Select(p => p).FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(Session["DeletedBackUpTble"])) ||
                    (!string.IsNullOrEmpty(Convert.ToString(Session["DeletedBackUpTble"])) &&
                    Convert.ToString(Session["DeletedBackUpTble"]) == "No"))
                {
                    //db.spDeleteBackUpTble();
                    db.spTakeBackUpBeforeLoanPost(objLoan.NextDueDt);
                    Session["DeletedBackUpTble"] = "Yes";
                }


                objLoanColsdb = db.Loan_Cols
                              .Where(p => p.LoanId == iLoanID)
                            .OrderByDescending(p => p.LoanColId)
                            .Select(p => p).FirstOrDefault();

                objMbr = db.Members
                                .Where(p => p.MbrId == objLoan.LoanID)
                                .Select(p => p).FirstOrDefault();

                objFinGroup = (from c in db.FinGroups
                               where c.GroupCode == objLoan.GrpCode
                               select c).FirstOrDefault();
                if (objFinGroup != null)
                    sGType = objFinGroup.GType;

                CurrentDue = objLoan.Prin_EMI;
                Savings = objLoan.ALRSavings;

                DateTime NxtDueDt = new DateTime(Convert.ToInt32(Convert.ToDateTime(objLoan.NextDueDt).Year), Convert.ToInt32(Convert.ToDateTime(objLoan.NextDueDt).Month), Convert.ToInt32(Convert.ToDateTime(objLoan.NextDueDt).Day));
                if (sGType.Trim() == "Daily")
                {
                    NxtDueDt = NxtDueDt.AddDays(1);
                }
                else
                {
                    NxtDueDt = NxtDueDt.AddDays(7);
                }

                //if (NxtDueDt.DayOfWeek.ToString().Trim() == "Sunday")
                //{
                //    NxtDueDt = NxtDueDt.AddDays(1);
                //}              

                objLoanColsdb.PostedUserID = Convert.ToString(Session["UserID"]);

                if (string.IsNullOrEmpty(sPostType) && sPostType.Trim() != "PrePaid")
                {
                    objLoanColsdb.LoanId = objLoanCols.LoanId;
                    objLoanColsdb.MbrId = objLoan.MbrId;

                    objLoanColsdb.Collected_Amount = Convert.ToString(Math.Round(Convert.ToDouble(CurrentDue))) == null ? "0" : Convert.ToString(Math.Round(Convert.ToDouble(CurrentDue)));


                    objLoanColsdb.Actual_Balance = Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Actual_Balance) - Convert.ToDouble(CurrentDue)));
                    objLoanColsdb.Actual_Balance = objLoanColsdb.Actual_Balance == null ? "0" : objLoanColsdb.Actual_Balance;

                    objLoanColsdb.Prin_Balance = Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Prin_Balance) - Convert.ToDouble(objLoan.Prin_EMI)));
                    objLoanColsdb.Prin_Balance = objLoanColsdb.Prin_Balance == null ? "0" : objLoanColsdb.Prin_Balance;

                    objLoanColsdb.Int_Collect = Convert.ToString(Convert.ToDouble(objLoan.Int_EMI));
                    objLoanColsdb.Int_Collect = objLoanColsdb.Int_Collect == null ? "0" : objLoanColsdb.Int_Collect;

                    objLoanColsdb.Prin_Collected = Convert.ToString(Convert.ToDouble(objLoan.Prin_EMI));
                    objLoanColsdb.Prin_Collected = objLoanColsdb.Prin_Collected == null ? "0" : objLoanColsdb.Prin_Collected;

                    objLoanColsdb.Balance_Installment = Convert.ToInt32(objLoanColsdb.Balance_Installment) - 1;
                    objLoanColsdb.Balance_Installment = objLoanColsdb.Balance_Installment == null ? 0 : objLoanColsdb.Balance_Installment;

                    objLoanColsdb.Balance_Interest = Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Balance_Interest) - Convert.ToDouble(objLoan.Int_EMI)));
                    objLoanColsdb.Balance_Interest = objLoanColsdb.Balance_Interest == null ? "0" : objLoanColsdb.Balance_Interest;

                    objLoanColsdb.Prin_Due = objLoan.Prin_EMI;
                    objLoanColsdb.Prin_Due = objLoanColsdb.Prin_Due == null ? "0" : objLoanColsdb.Prin_Due;

                    objLoanColsdb.Int_Due = objLoan.Int_EMI;
                    objLoanColsdb.Int_Due = objLoanColsdb.Int_Due == null ? "0" : objLoanColsdb.Int_Due;

                    objLoanColsdb.Transact_Date = Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Year);
                    objLoanColsdb.Transact_Date = objLoanColsdb.Transact_Date == null ? "0" : objLoanColsdb.Transact_Date;

                    objLoanColsdb.Next_Due_Date = NxtDueDt.ToShortDateString();// Convert.ToString(NxtDueDt.Day) + "/" + Convert.ToString(NxtDueDt.Month) + "/" + Convert.ToString(NxtDueDt.Year);
                    objLoanColsdb.Next_Due_Date = objLoanColsdb.Next_Due_Date == null ? "0" : objLoanColsdb.Next_Due_Date;

                    objLoanColsdb.Upto_Last_Savings = objLoanCols.Upto_Last_Savings;
                    objLoanColsdb.Upto_Last_Savings = objLoanColsdb.Upto_Last_Savings == null ? "0" : objLoanColsdb.Upto_Last_Savings;

                    objLoanColsdb.ALRSavings = Convert.ToString(Savings);  // Convert.ToString(Math.Round(Convert.ToDouble(objLoanCols.During_Savings) + Convert.ToDouble(Savings)));
                    objLoanColsdb.ALRSavings = objLoanColsdb.ALRSavings == null ? "0" : objLoanColsdb.ALRSavings;

                    objLoanColsdb.As_On = objLoanCols.As_On;
                    objLoanColsdb.As_On = objLoanColsdb.As_On == null ? "0" : objLoanColsdb.As_On;

                    objLoanColsdb.Adjustment = objLoanCols.Adjustment;
                    objLoanColsdb.Adjustment = objLoanColsdb.Adjustment == null ? "0" : objLoanColsdb.Adjustment;

                    objLoanColsdb.PaidEMI = Convert.ToString(Convert.ToInt32(Math.Round(Convert.ToDouble(objLoanColsdb.PaidEMI)) + 1));
                    objLoanColsdb.PaidEMI = objLoanColsdb.PaidEMI == null ? "0" : objLoanColsdb.PaidEMI;

                    #region ALR Savings
                    //string sALRMbr = db.ALRAudjustments
                    // .Where(p => p.MbrID == objLoan.MbrId)
                    // .Select(p => p.AdvanceALR).FirstOrDefault();

                    //if (!string.IsNullOrEmpty(sALRMbr) && Convert.ToDouble(sALRMbr) > 0)
                    //{
                    //    ALRAudjustment onbALRAudjustments = new ALRAudjustment();
                    //    onbALRAudjustments.AdvanceALR = (Convert.ToDouble(onbALRAudjustments.AdvanceALR) - Convert.ToDouble(CurrentDue)).ToString();

                    //    db.Entry(onbALRAudjustments).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}

                    //decimal dNetSavings = 0;
                    //var vDuringSavings = (from p in db.Loan_Cols
                    //                      where p.MbrId == objLoan.MbrId
                    //                      select p.ALRSavings);

                    //foreach (var vVal in vDuringSavings)
                    //{
                    //    if (!string.IsNullOrEmpty(Convert.ToString(vVal)))
                    //    {
                    //        dNetSavings += Convert.ToDecimal(vVal);
                    //    }
                    //}

                    //if (objLoanColsdb.Adjustment != null)
                    //{
                    //    dNetSavings = dNetSavings - Convert.ToDecimal(objLoanColsdb.Adjustment);
                    //}
                    //objLoan.NetSavings = objLoan.NetSavings == "undefined" ? "0" : objLoan.NetSavings;
                    //objLoan.NetSavings = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.NetSavings) + Convert.ToDouble(Savings)));
                    //objLoan.NetSavings = objLoan.NetSavings == null ? "0" : objLoan.NetSavings;
                    #endregion

                    objLoan.Balance_Amount = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.Balance_Amount) - Convert.ToDouble(Convert.ToDouble(objLoan.Prin_EMI))));
                    objLoan.Balance_Amount = objLoan.Balance_Amount == null ? "0" : objLoan.Balance_Amount;

                    objLoan.NextDueDt = NxtDueDt.ToShortDateString();
                    objLoan.Balance_Interest = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.Balance_Interest) - Convert.ToDouble(objLoan.Int_EMI)));
                    objLoan.Balance_Interest = objLoan.Balance_Interest == null ? "0" : objLoan.Balance_Interest;

                    db.Loan_Cols.Add(objLoanColsdb);
                    db.SaveChanges();

                    db.Entry(objLoan).State = EntityState.Modified;
                    db.SaveChanges();

                    if (objLoan.Balance_Amount == "0.00")
                    {
                        objMbr.MbrStatus = "Cancel";
                        db.Entry(objMbr).State = EntityState.Modified;
                        db.SaveChanges();

                        objFinGroup.GStatus = "Cancel";
                        db.Entry(objFinGroup).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    sIsSucess = "1";

                    logger.Info(Session["UserID"] + ": Posted today recovery at th time of : " + DateTime.Now);
                }
                else
                {
                    objLoanColsdb.PrePrinAmt = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.Balance_Amount) - Convert.ToDouble(objLoanColsdb.Prin_Due)));

                    objLoanColsdb.PreInterestAmt = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.Balance_Interest) - Convert.ToDouble(objLoanColsdb.Int_Due)));
                    objLoanColsdb.Collect_Or_RefoundAmt = sAdjustment;
                    objLoanColsdb.Adjustment = objLoan.NetSavings;

                    objLoanColsdb.LoanId = objLoanCols.LoanId;
                    objLoanColsdb.MbrId = objLoan.MbrId;
                    objLoanColsdb.Collected_Amount = Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Int_Due) + Convert.ToDouble(objLoanColsdb.Prin_Due)));
                    objLoanColsdb.Actual_Balance = "0.00"; // Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Actual_Balance) - Convert.ToDouble(CurrentDue)));
                    objLoanColsdb.Prin_Balance = "0.00"; // Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Prin_Balance) - Convert.ToDouble(objLoanColsdb.Prin_Balance)));
                    objLoanColsdb.Int_Collect = Convert.ToString(Convert.ToDouble(objLoan.Int_EMI)); // Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Int_Collect) + Convert.ToDouble(objLoanColsdb.Balance_Interest)));
                    objLoanColsdb.Prin_Collected = Convert.ToString(Convert.ToDouble(objLoan.Prin_EMI)); // Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Prin_Collected) + Math.Round(Convert.ToDouble(CurrentDue) - Convert.ToDouble(objLoanColsdb.Balance_Interest))));

                    objLoanColsdb.Balance_Interest = "0.00"; //  Convert.ToString(Math.Round(Convert.ToDouble(objLoanColsdb.Balance_Interest) - Convert.ToDouble(objLoanColsdb.Balance_Interest)));
                    objLoanColsdb.Prin_Due = "0.00"; //Convert.ToString(Convert.ToDouble(objLoan.Prin_EMI));
                    objLoanColsdb.Int_Due = "0.00"; // Convert.ToString(Convert.ToDouble(objLoan.Int_EMI));
                    objLoanColsdb.Transact_Date = Convert.ToString(DateTime.Now.ToShortDateString());
                    objLoanColsdb.Next_Due_Date = string.Empty;// NxtDueDt.ToShortDateString(); 
                    objLoanColsdb.Upto_Last_Savings = objLoanCols.Upto_Last_Savings;
                    objLoanColsdb.ALRSavings = "0.00"; // Convert.ToString(Math.Round(Convert.ToDouble(objLoanCols.During_Savings) + Convert.ToDouble(Savings)));
                    objLoanColsdb.As_On = "0.00"; //objLoanCols.As_On;
                    objLoanColsdb.PaidEMI = Convert.ToString(Convert.ToInt32(objLoanColsdb.PaidEMI) + Convert.ToInt32(objLoanColsdb.Balance_Installment));
                    objLoanColsdb.Balance_Installment = 0;// Convert.ToInt32(objLoanColsdb.Balance_Installment) - Convert.ToInt32(objLoanColsdb.Balance_Installment);

                    objLoan.Balance_Amount = "0.00";
                    objLoan.NextDueDt = string.Empty;
                    objLoan.Balance_Interest = Convert.ToString(Math.Round(Convert.ToDouble(objLoan.Balance_Interest) - Convert.ToDouble(Convert.ToDouble(objLoan.Balance_Interest)))); ;
                    objLoan.Expiry_Date = Convert.ToString(DateTime.Now.ToShortDateString());
                    db.Loan_Cols.Add(objLoanColsdb);
                    db.SaveChanges();

                    //db.Entry(objLoan).State = EntityState.Modified;
                    //db.SaveChanges();

                    //objFinGroup.GStatus = "Cancel";
                    //db.Entry(objFinGroup).State = EntityState.Modified;
                    //db.SaveChanges();

                    //objMbr.MbrStatus = "Cancel";
                    //db.Entry(objMbr).State = EntityState.Modified;
                    //db.SaveChanges();

                    sIsSucess = "1";

                    logger.Info(Session["UserID"] + ": Posted today recovery at th time of : " + DateTime.Now);
                }
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoanPosting() Get" + ex.InnerException);

            }
            return sIsSucess;
            //return Json(LoanId, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string GetLoanDetails(int iLoanID)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            LoanCols objLoanCols = new LoanCols();
            string sOutPut = string.Empty;
            try
            {
                string sLoanIDs = Convert.ToString(Session["LoansIDs"]);
                if (!string.IsNullOrEmpty(Convert.ToString(iLoanID)))
                {
                    Loan objMbr = new Loan();
                    objMbr = (from c in db.Loans
                              where c.LoanID == iLoanID
                              select c).FirstOrDefault();
                    int iMbrID = Convert.ToInt32(objMbr.MbrId);

                    var result = db.spGetLoanDetails(iLoanID, iMbrID).FirstOrDefault();
                    sOutPut += result.LoanId + "//";
                    sOutPut += result.MbrId + "//";
                    sOutPut += Convert.ToString(Math.Round(Convert.ToDouble(objMbr.Prin_EMI) + Convert.ToDouble(objMbr.Int_EMI))) + "//"; // result.Collected_Amount + "//";
                    sOutPut += result.Actual_Balance + "//";
                    sOutPut += result.Prin_Balance + "//";
                    sOutPut += objMbr.Int_EMI + "//"; // result.Int_Collect + "//";
                    sOutPut += objMbr.Prin_EMI + "//"; // result.Prin_Collected + "//";
                    sOutPut += result.Balance_Installment + "//";
                    sOutPut += result.Balance_Interest + "//";
                    sOutPut += result.Prin_Due + "//";
                    sOutPut += result.Int_Due + "//";
                    sOutPut += result.Transact_Date + "//";
                    sOutPut += result.Next_Due_Date + "//";
                    sOutPut += result.Upto_Last_Savings + "//";
                    sOutPut += result.During_Savings + "//";
                    sOutPut += result.As_On + "//";
                    sOutPut += result.Adjustment + "//"; ;

                    FinGroup objGrp = new FinGroup();
                    objGrp = (from c in db.FinGroups
                              where c.GroupCode == objMbr.GrpCode
                              select c).FirstOrDefault();
                    sOutPut += objGrp.GType;
                    return sOutPut;
                }
                else
                {
                    return sOutPut;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetLoanDetails() Get" + ex.InnerException);
                return sOutPut;
            }
        }

        public string GetLoanIDFromView(string sLoanID)
        {
            if (Convert.ToInt32(sLoanID) > 0)
            {
                Session["LoansIDs"] = sLoanID;
                return LoanPosting(string.Empty, sLoanID, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                //return true;
            }
            else { return "False"; }
        }

        [HttpGet]
        public ActionResult WithdrawDateScreen()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                List<Member> objMbr = new List<Member>();


                var data = from c in db.Loans
                           where c.Balance_Amount == "0" || c.Balance_Amount == null
                           select c;


                int MbrId = 0;
                foreach (var sval in data.ToArray())
                {
                    Loan NewObj = new Loan();
                    NewObj = (Loan)sval;

                    MbrId = Convert.ToInt32(NewObj.MbrId);
                    Member Mbr = new Member();
                    Mbr = (from c in db.Members
                           where c.MbrId == MbrId
                           select c).FirstOrDefault();

                    objMbr.Add(Mbr);
                }

                return View(objMbr);
            }
            else
            {
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        [HttpPost]
        public ActionResult WithdrawDateScreen(MemberViewModel objMbr)
        {
            return View();
        }

        [HttpGet]
        public JsonResult UpdateMemberWDt(string UserModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserModel))
                {
                    using (MicroFinanceEntities db = new MicroFinanceEntities())
                    {
                        int iMbrID = Convert.ToInt32(UserModel.Split(',')[0]);
                        Member objMbr = (from c in db.Members
                                         where c.MbrId == iMbrID
                                         select c).FirstOrDefault();
                        objMbr.WithdrawDt = UserModel.Split(',')[1];
                        db.Entry(objMbr).State = EntityState.Modified;
                        db.SaveChanges();
                        logger.Info(Session["UserID"] + ": Updated member withdraw date of this member  " + objMbr.MbrName + " :" + DateTime.Now);
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in UpdateMemberWDt() Get" + ex.InnerException);
                return Json("No", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public string LoanLaterDisbus(string iMbrID)
        {
            Loan objLoan = new Loan();
            int iMemberID = Convert.ToInt32(iMbrID);
            string IsLoanDisbused = string.Empty, sExistingLoanType = string.Empty;
            try
            {
                using (MicroFinanceEntities db = new MicroFinanceEntities())
                {
                    objLoan = db.Loans
                                .Where(p => p.MbrId == iMemberID)
                                .OrderByDescending(p => p.LoanID)
                                .Select(p => p).FirstOrDefault();

                    Session["LoanLaterDisbusMbrID"] = iMbrID;

                    if (objLoan != null && string.IsNullOrEmpty(objLoan.Expiry_Date))
                    {
                        sExistingLoanType = db.Loans
                         .Where(p => p.MbrId == iMemberID && p.LoanType == objLoan.LoanType)
                         .Select(p => p.LoanType).FirstOrDefault();


                        if (!string.IsNullOrEmpty(sExistingLoanType))
                        {
                            IsLoanDisbused = "No";
                        }
                        else
                        {
                            IsLoanDisbused = "Yes";
                        }
                    }
                    else
                    {
                        IsLoanDisbused = "No";
                    }
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error in LoanLaterDisbus() Get" + objEx.InnerException);
            }
            return IsLoanDisbused;
        }

        [HttpGet]
        public ActionResult PLoanLists(string GrpCodeID, string MbrCode)
        {
            List<Loan> objLoan = new List<Loan>();
            MicroFinanceEntities db = new MicroFinanceEntities();
            int iMbrID = string.IsNullOrEmpty(MbrCode) ? 0 : Convert.ToInt32(MbrCode);

            if (iMbrID == 0 && string.IsNullOrEmpty(GrpCodeID))
            {
                objLoan = db.Loans
               .OrderByDescending(p => p.LoanID)
               .Select(p => p).ToList();
            }
            else if (iMbrID == 0 && !string.IsNullOrEmpty(GrpCodeID))
            {
                objLoan = db.Loans
                   .Where(p => p.GrpCode == GrpCodeID)
                   .OrderByDescending(p => p.LoanID)
                   .Select(p => p).ToList();
            }
            else if (iMbrID > 0 && !string.IsNullOrEmpty(GrpCodeID))
            {
                objLoan = db.Loans
                   .Where(p => p.MbrId == iMbrID && p.GrpCode == GrpCodeID)// (p.Expiry_Date == string.Empty || p.Expiry_Date == null || p.Expiry_Date == "") && )
                   .OrderByDescending(p => p.LoanID)
                   .Select(p => p).ToList();
            }
            else if (iMbrID > 0)
            {
                objLoan = db.Loans
                   .Where(p => p.MbrId == iMbrID)
                   .OrderByDescending(p => p.LoanID)
                   .Select(p => p).ToList();
            }

            return PartialView("_MemberList", objLoan);
        }

        [HttpGet]
        public ActionResult LoanLists(string GrpCodeID)
        {
            List<Loan> objLoan = new List<Loan>();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();

                    if (!string.IsNullOrEmpty(GrpCodeID))
                    { 
                        if (GrpCodeID.ToLower() == "undefined")
                        {
                            GrpCodeID = string.Empty;
                        }

                        ViewBag.GrpCodeID = GrpCodeID; 

                        if (string.IsNullOrEmpty(GrpCodeID))
                        {
                            objLoan = db.Loans
                           .Where(p => p.Status == 0)
                           .OrderByDescending(p => p.LoanID)
                           .Select(p => p).ToList();
                        }
                        else if (!string.IsNullOrEmpty(GrpCodeID))
                        {
                            objLoan = db.Loans
                               .Where(p => p.Status == 0
                               && p.GrpCode == GrpCodeID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).ToList();
                        }
                        else if (!string.IsNullOrEmpty(GrpCodeID))
                        {                            
                            objLoan = db.Loans
                               .Where(p => p.GrpCode == GrpCodeID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).ToList();
                        }
                        else if (string.IsNullOrEmpty(GrpCodeID))
                        {
                            objLoan = db.Loans
                               .Where(p => p.GrpCode == GrpCodeID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).ToList();
                        }
                        else
                        {
                            objLoan = db.Loans
                                        .Where(p => p.Status == 0)
                                        .OrderByDescending(p => p.LoanID)
                                        .Select(p => p).ToList();
                        }

                    }
                    else
                    {
                        objLoan = db.Loans
                                    .Where(p => p.Status == 0)
                                    .OrderByDescending(p => p.LoanID)
                                    .Select(p => p).ToList();

                        Session["GrpCodeID"] = null;
                        Session["MbrCode"] = null;
                    }

                    #region UI binding

                    var GrpList = db.FinGroups.AsEnumerable().ToList();

                    IEnumerable<SelectListItem> GrpCodeList = (from p in GrpList.AsEnumerable()
                                                               select new SelectListItem
                                                               {
                                                                   Text = p.GrpName,
                                                                   Value = p.GroupCode
                                                               }).ToList();
                    ViewBag.GrpCodeList = GrpCodeList;

                    objLoan.ForEach(rec =>
                    {
                        rec.GrpCode = GrpList.Where(x => x.GroupCode == rec.GrpCode).Select(p => p.GrpName).FirstOrDefault();
                    });

                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffID.ToString()
                                                      }).ToList();

                    ViewBag.StfList = StaffList;
                    #endregion
                    ViewBag.LoanList = objLoan;
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in LoanLists() Get" + ex.InnerException);
            }
            return View(objLoan);
        }

        [HttpGet]
        public ActionResult ALRAdjustments()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
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
                logger.Error("Error in MemberDetails() Get Method" + ex.InnerException);
                ModelState.AddModelError(string.Empty, "Error while saving member");
                return View();
            }
        }

        [HttpGet]
        public JsonResult ALRAdjust(string iMbrId, string ALRAmt, string ALRType)
        {
            Loan objLoan = new Loan();
            try
            {
                if (!string.IsNullOrEmpty(iMbrId))
                {
                    using (MicroFinanceEntities db = new MicroFinanceEntities())
                    {
                        int iMbrID = Convert.ToInt32(iMbrId);

                        if (ALRType.Trim() == "ALRSavings")
                        {
                            objLoan = (from c in db.Loans
                                       where c.MbrId == iMbrID
                                       select c).FirstOrDefault();
                            objLoan.ALRSavings = ALRAmt;


                            db.Entry(objLoan).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            ALRAudjustment objALRAudjustments = new ALRAudjustment();
                            objALRAudjustments.MbrID = Convert.ToInt32(iMbrId);
                            objALRAudjustments.AdvanceALR = ALRAmt;
                            objALRAudjustments.CrDt = DateTime.Now.ToShortDateString();

                            db.Entry(objALRAudjustments);
                            db.SaveChanges();
                        }
                        logger.Info(Session["UserID"] + ": Updated member ALR details of this member  " + objLoan.MbrId + " :" + DateTime.Now);
                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in UpdateMemberWDt() Get" + ex.InnerException);
                return Json("No", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTodayALRMbrList(string sGrpCode, string sStaffId)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (string.IsNullOrEmpty(sGrpCode) || sGrpCode.Trim() == "--All Groups--")
                {
                    sGrpCode = null;
                }

                if (string.IsNullOrEmpty(sStaffId) || sStaffId.Trim() == "--All Staff--")
                {
                    sStaffId = null;
                }

                // return Json(db.spGetLoanMbrsList(sGrpCode, sStaffId).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansList () Get" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Rollback()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            Loan objLoan = new Loan();
            Loan_Cols objLoan_Cols = new Loan_Cols();
            List<Loan> objListLoan = new List<Loan>();
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) == "Admin")
                {
                    logger.Info("Opened from this user : " + Session["UserID"]);
                    if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                    {
                        foreach (TmpLoansBackUpData obj in db.TmpLoansBackUpDatas.ToArray())
                        {
                            TmpLoansBackUpData newobj = new TmpLoansBackUpData();
                            AutoMapper.Mapper.CreateMap<TmpLoansBackUpData, Loan>();
                            objLoan = AutoMapper.Mapper.Map<TmpLoansBackUpData, Loan>(obj);

                            db.Entry(objLoan).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        foreach (TmpLoanColsBackUpData objLoanColId in db.TmpLoanColsBackUpDatas.ToArray())
                        {
                            if (objLoanColId.LoanColId > 0)
                            {
                                objLoan_Cols = (from c in db.Loan_Cols
                                                where c.LoanColId == objLoanColId.LoanColId
                                                select c).FirstOrDefault();
                                db.Entry(objLoan_Cols).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                        }


                        return View();
                    }
                    else
                    {
                        return RedirectToAction("StaffLogin", "Staff");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Charts");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansList () Get" + ex.InnerException);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditLoan(LoanDisbus objLoanDisb)
        {

            Loan objLoan = new Loan();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) != "Admin")
                    {
                        return Json("RoleIssue", JsonRequestBehavior.AllowGet);
                    }

                    if (objLoanDisb != null)
                    {
                        MicroFinanceEntities db = new MicroFinanceEntities();

                        int iLoanID = objLoanDisb.LoanID;

                        objLoan = db.Loans
                                    .Where(p => p.LoanID == objLoanDisb.LoanID)
                                    .Select(p => p).FirstOrDefault();

                        objLoan.Loan_Amount = objLoanDisb.Loan_Amount;
                        objLoan.Balance_Amount = objLoanDisb.Balance_Amount;
                        objLoan.Balance_Amount = objLoan.Balance_Amount == null ? "0" : objLoan.Balance_Amount;

                        objLoan.Prin_EMI = string.IsNullOrEmpty(objLoanDisb.Prin_EMI) ? "0" : objLoanDisb.Prin_EMI;
                        objLoan.Prin_EMI = objLoan.Prin_EMI == null ? "0" : objLoan.Prin_EMI;

                        objLoan.Int_EMI = string.IsNullOrEmpty(objLoanDisb.Int_EMI) ? "0" : objLoanDisb.Int_EMI;
                        objLoan.Int_EMI = objLoan.Int_EMI == null ? "0" : objLoan.Int_EMI;

                        objLoan.NoOfDay = objLoanDisb.NoOfDays == 0 ? 0 : Convert.ToInt32(objLoanDisb.NoOfDays);
                        objLoan.NoOfDay = objLoan.NoOfDay == null ? 0 : objLoan.NoOfDay;

                        objLoan.Date_Of_Disbursement = string.IsNullOrEmpty(objLoanDisb.Date_Of_Disbursement) ? DateTime.Today.ToShortDateString() : objLoanDisb.Date_Of_Disbursement;
                        objLoan.NextDueDt = string.IsNullOrEmpty(objLoanDisb.NextDueDt) ? DateTime.Today.ToShortDateString() : objLoanDisb.NextDueDt;
                        objLoan.StaffName = string.IsNullOrEmpty(objLoanDisb.StaffName) ? "0" : objLoanDisb.StaffName;
                        objLoan.GrpCode = string.IsNullOrEmpty(objLoanDisb.GrpName) ? "0" : objLoanDisb.GrpName;

                        objLoan.NetSavings = objLoanDisb.Savings;
                        objLoan.NetSavings = objLoan.NetSavings == null ? "0" : objLoan.NetSavings;

                        objLoan.Balance_Interest = objLoanDisb.RateOfInterest;
                        objLoan.Balance_Interest = objLoan.Balance_Interest == null ? "0" : objLoan.Balance_Interest;
                        objLoan.Status = string.IsNullOrEmpty(objLoanDisb.Expiry_Date) ? Convert.ToDateTime(objLoanDisb.Expiry_Date) <= DateTime.Now ? 1 : 0 : 0;
                        db.Entry(objLoan).State = EntityState.Modified;
                        db.SaveChanges();

                        Session["Edited"] = "Yes";

                        
                    }
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditLoan() Get" + ex.InnerException);  
            }
            return RedirectToAction("EditLoan", "Loans", new { iLoanId = objLoan.LoanID });
        }

        [HttpGet]
        public ActionResult EditLoan(int iLoanId)
        {
            try
            {
                ViewBag.IsAdmin = Helper.IsAdmin;
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    LoanDisbus objLoanDisbus = new LoanDisbus();

                    Loan objDBLoans = new Loan();
                    objDBLoans = db.Loans.Where(p => p.LoanID == iLoanId && p.Status == 0).FirstOrDefault();

                    objLoanDisbus.LoanID = objDBLoans.LoanID;
                    objLoanDisbus.MbrId = (int)objDBLoans.MbrId;
                    objLoanDisbus.Loan_Amount = objDBLoans.Loan_Amount;
                    objLoanDisbus.Balance_Amount = objDBLoans.Balance_Amount;
                    objLoanDisbus.Prin_EMI = objDBLoans.Prin_EMI;
                    objLoanDisbus.Int_EMI = objDBLoans.Int_EMI;
                    objLoanDisbus.RateOfInterest = objDBLoans.RateOfInterest;
                    objLoanDisbus.Date_Of_Disbursement = objDBLoans.Date_Of_Disbursement;
                    objLoanDisbus.Expiry_Date = objDBLoans.Expiry_Date;
                    objLoanDisbus.NextDueDt = objDBLoans.NextDueDt;
                    objLoanDisbus.Processing_Fee = objDBLoans.Processing_Fee;
                    objLoanDisbus.GRFAmt = objDBLoans.GRFAmt;

                    objLoanDisbus.Insurance = (int)objDBLoans.Insurance;
                    objLoanDisbus.Other_Income = objDBLoans.Other_Income;
                    objLoanDisbus.Savings = objDBLoans.ALRSavings;
                    objLoanDisbus.NoOfDays = (int)objDBLoans.NoOfDay;
                    objLoanDisbus.CrD = objDBLoans.Date_Of_Disbursement.Split('/')[1];
                    objLoanDisbus.CrM = objDBLoans.Date_Of_Disbursement.Split('/')[0];
                    objLoanDisbus.CrY = objDBLoans.Date_Of_Disbursement.Split('/')[2];
                    objLoanDisbus.SelectedGrpName = string.IsNullOrEmpty(objDBLoans.GrpCode) ? "--All Groups--" : objDBLoans.GrpCode;
                    objLoanDisbus.SelectedMbrName = string.IsNullOrEmpty(objDBLoans.StaffName) ? "--All Members--" : objDBLoans.StaffName;
                    objLoanDisbus.LoanType = objDBLoans.LoanType;


                    if (!string.IsNullOrEmpty(Convert.ToString(Session["Edited"])) && Convert.ToString(Session["Edited"]) == "Yes")
                    {
                            objLoanDisbus.Savings = "Updated";
                    }

                    return View(objLoanDisbus);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in LoansDisbus () Get" + ex.InnerException);
            }
            return View();

        }

        [HttpGet]
        public JsonResult GetLoanMemberList(string sGrpCode)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;

            try
            {


                if ((sGrpCode == null || sGrpCode == "" || sGrpCode == "--All Groups--"))
                {
                    return Json(db.Members.ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = db.Members
                                .Where(x => x.GroupCode == sGrpCode)
                                .Select(n => new { n.MbrId, n.MbrName, });

                    List<SelectListItem> MbrList = (from p in db.Members.Where(x => x.GroupCode == sGrpCode).AsEnumerable()
                                                    select new SelectListItem
                                                    {
                                                        Text = p.MbrName,
                                                        Value = p.MbrId.ToString()
                                                    }).ToList();


                    return Json(MbrList.ToList(), JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditGroup() Get" + ex.InnerException);
                return Json("No", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PartialIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PostHistry()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Partialload(string TabName, string GrpID)
        {
            string sPViewName = string.Empty;
            List<Loan> objLoan = new List<Loan>();
            List<Member> objMbr = new List<Member>();
            List<Loan_Cols> objLoan_Cols = new List<Loan_Cols>();

            int iLoanID = 0;

            MicroFinanceEntities db = new MicroFinanceEntities();
            ViewBag.IsAdmin = Helper.IsAdmin;
            if (!string.IsNullOrEmpty(TabName))
            {
                if (TabName.Contains("LoansList"))
                {
                    sPViewName = "_LoanList";

                    if (TabName.Split(',').Length > 1)
                    {
                        int iMbrID = Convert.ToInt32(TabName.Split(',')[1]);

                        if (iMbrID == 0 && string.IsNullOrEmpty(GrpID))
                        {
                            objLoan = db.Loans
                           .Where(p => p.Expiry_Date == string.Empty || p.Expiry_Date == null || p.Expiry_Date == "")
                           .OrderByDescending(p => p.LoanID)
                           .Select(p => p).ToList();
                        }
                        else if (iMbrID == 0 && !string.IsNullOrEmpty(GrpID))
                        {
                            objLoan = db.Loans
                               .Where(p => (p.Expiry_Date == string.Empty || p.Expiry_Date == null || p.Expiry_Date == "")
                               && p.GrpCode == GrpID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).ToList();
                        }
                        else if (iMbrID > 0 && !string.IsNullOrEmpty(GrpID))
                        {

                            objLoan = db.Loans
                               .Where(p => (p.Expiry_Date == string.Empty || p.Expiry_Date == null || p.Expiry_Date == "")
                               && p.LoanID == iMbrID && p.GrpCode == GrpID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).ToList();
                        }
                        else if (iMbrID > 0)
                        {
                            Loan objLoanM = new Loan();
                            Member objMbrd = new Member();
                            objLoanM = db.Loans
                               .Where(p => p.MbrId == iMbrID)
                               .OrderByDescending(p => p.LoanID)
                               .Select(p => p).FirstOrDefault();

                            objMbrd = db.Members
                               .Where(p => p.MbrId == iMbrID)
                               .Select(p => p).FirstOrDefault();

                            Loan_Cols objLoan_Colsm = new Loan_Cols();
                            objLoan_Colsm.Loan = objLoanM;
                            objLoan_Colsm.Member = objMbrd;
                            string imgName = "PLogo.png";

                            ViewBag.ImagePath = @"~/Resource/" + imgName;
                            return PartialView(sPViewName, objLoan_Colsm);
                        }

                    }
                    else
                    {

                        objLoan = db.Loans
                                .Where(p => p.Expiry_Date == string.Empty || p.Expiry_Date == null || p.Expiry_Date == "")
                                .OrderByDescending(p => p.LoanID)
                                .Select(p => p).ToList();
                    }



                    return PartialView(sPViewName, objLoan);

                }
                else if (TabName == "MbrList")
                {
                    sPViewName = "_MemberList";
                    objMbr = db.Members.ToList();

                    return PartialView(sPViewName, objMbr);

                }
                else if (TabName.Contains("HistList"))
                {

                    if (TabName.Split(',').Length > 1)
                    {
                        iLoanID = Convert.ToInt32(TabName.Split(',')[1]);
                        objLoan_Cols = db.Loan_Cols.Where(x => x.LoanId == iLoanID).OrderByDescending(x => x.LoanColId).ToList();
                        //return View("PostHistry", objLoan_Cols);                        
                    }
                    else
                    {
                        ViewBag.IsFrmTab = "Yes";
                        objLoan_Cols = db.Loan_Cols.OrderByDescending(x => x.LoanColId).OrderByDescending(x => x.Transact_Date).ToList();
                    }

                    sPViewName = "_PostingHistory";
                    return PartialView(sPViewName, objLoan_Cols);
                }
                else
                {

                }
            }
            return PartialView(sPViewName, string.Empty);
        }


        [HttpGet]
        public JsonResult RemoveLoan(int iLoanID)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(iLoanID)))
                {
                    Loan objLoan = new Loan();

                    objLoan = db.Loans.Where(c => c.LoanID == iLoanID).FirstOrDefault();


                    db.Loans.Remove(objLoan);
                    db.SaveChanges();
                    logger.Info(Session["UserID"] + ": removed this member " + objLoan.LoanID + DateTime.Now);
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in RemoveLoan() Get Method" + ex.InnerException);
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult LoanLists_New(string GrpCodeID = "")
        {
            var result = LoanLists(GrpCodeID);
            if (result is ViewResult viewResult)
            {
                viewResult.ViewName = "LoanLists_New";
            }
            return result;
        }

        [HttpGet]
        public ActionResult LoansDisbus_New(int iMbrID = 0)
        {
            var result = LoansDisbus(iMbrID);
            if (result is ViewResult viewResult)
            {
                viewResult.ViewName = "LoansDisbus_New";
            }
            return result;
        }

        [HttpPost]
        public ActionResult LoansDisbus_New(LoanDisbus model)
        {
            var result = LoansDisbus(model);
            if (result is ViewResult viewResult)
            {
                viewResult.ViewName = "LoansDisbus_New";
            }
            return result;
        }

    }
}
//--ALTER TABLE Member
//--ADD PhoneNo2 varchar(12);//--ADD PhoneNo2 varchar(12);