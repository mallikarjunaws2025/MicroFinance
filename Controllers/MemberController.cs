using TestApp.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TestApp.Comman;
using TestApp.VModels.Member;


namespace TestApp.Controllers
{
    public class MemberController : Controller
    {
        public MemberController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        [HttpGet]
        public ActionResult CreateMember()
        {
            MemberViewModel objMbr = new MemberViewModel();
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
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
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.StaffMbrList = StaffList;
                    objMbr.GrpNameList = GrpCodeList;
                    objMbr.IsSucess = "0";
                    return View(objMbr);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in CreateMember() get Method" + ex.InnerException);
                ModelState.AddModelError(string.Empty, "Error while loading page");
                return View(objMbr);
            }
        }

        [HttpPost]
        public ActionResult CreateMember(MemberViewModel objMbr)
        {
            FinGroup objGrp = new FinGroup();
            Member objCrMbr = new Member();
            int iStaffID = 0;
            string sStaffName = string.Empty;
            string sRetCode = string.Empty;
            try
            {
                MicroFinanceEntities db = new MicroFinanceEntities();

                if (db.Members.Where(c => c.AdharaCardNum == objMbr.AdharaCardNum.Trim()).ToList().Count > 0)
                {
                    sRetCode = "A";
                }

               

                objMbr.IsSucess = sRetCode;

                if (string.IsNullOrEmpty(sRetCode))
                {

                    objGrp = (from c in db.FinGroups
                              where c.GroupCode == objMbr.GrpCode
                              select c).FirstOrDefault();

                    TempData["GType"] = objGrp.GType;
                    TempData.Keep("GType");

                    iStaffID = Convert.ToInt32(objMbr.StaffID);

                    objCrMbr.MbrName = objMbr.MbrName;
                    objCrMbr.GroupCode = objGrp.GroupCode;
                    objCrMbr.GrpName = objGrp.GrpName;
                    objCrMbr.Husbandname = objMbr.HsbndName;
                    objCrMbr.Age = objMbr.Age;
                    objCrMbr.MbrStatus = objMbr.MbrStatus;
                    objCrMbr.Gender = objMbr.Gen;
                    objCrMbr.CantactNo = objMbr.CantactNum;                    
                    objCrMbr.DOJ = objMbr.MbrDOJ; //Convert.ToString(objMbr.CrM + "/" + objMbr.CrD + "/" + objMbr.CrY);
                    objCrMbr.WithdrawDt = string.Empty; // Convert.ToString(DateTime.Now); // Convert.ToDateTime(objMbr.CrD + "/" + objMbr.CrM + "/" + objMbr.CrY);
                    objCrMbr.StaffId = objMbr.StaffID;
                    objCrMbr.AdharaCardNum = objMbr.AdharaCardNum.Trim();

                    objCrMbr.Nominee = objMbr.Nominee;
                    objCrMbr.PhoneNo2 = objMbr.PhoneNo2;

                    sStaffName = db.Staffs
                          .Where(p => p.StaffID == iStaffID)
                          .Select(p => p.StaffName).FirstOrDefault();

                    objCrMbr.StaffName = sStaffName;
                    objCrMbr.MbrAddress = objMbr.MbrAddress;
                    objCrMbr.RCardNo = objMbr.RationCardNum;

                    db.Members.Add(objCrMbr);
                    db.SaveChanges();
                    int id = objCrMbr.MbrId;
                    logger.Info(Session["UserID"] + ": one loan disbused for Member name : " + objMbr.MbrName + " : " + DateTime.Now);

                    objMbr.IsSucess = "1";
                    Session["SavedMbrID"] = db.Members.Max(x => (int?)x.MbrId) ?? 0;
                    objMbr.MbrName = objCrMbr.MbrId.ToString();
                    //Session["MemberDetails"] = objMbr;
                    logger.Info(Session["UserID"] + ": created member : time :" + DateTime.Now);
                    //return RedirectToAction("LoansDisbus", "Loans");
                }
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
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.StaffMbrList = StaffList;
                    objMbr.GrpNameList = GrpCodeList;

                    return View(objMbr);
                
            }
            catch (Exception ex)
            {
                logger.Error("Error in CreateMember() Post Method" + ex.InnerException);
                objMbr.IsSucess = "2";
                ModelState.AddModelError(string.Empty, "Error while saving member");
                return View(objMbr);
            }

        }

        [HttpGet]
        public ActionResult MemberDetails()
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
        public JsonResult MemberList(string sGrpCode, string sStaffId)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<Member> objMbr = new List<Member>();
            try
            {
                int iStaffId = 0;
                if (!string.IsNullOrEmpty(sStaffId))
                    iStaffId = Convert.ToInt32(sStaffId);

                ViewBag.IsAdmin = Helper.IsAdmin;

                // Normalize the group code - handle various "all" representations
                bool isAllGroups = string.IsNullOrEmpty(sGrpCode) || 
                                  sGrpCode == "--All Groups--" || 
                                  sGrpCode == "-- All Groups --" ||
                                  sGrpCode.Trim() == "";

                // Handle staff ID properly
                bool isAllStaff = iStaffId <= 0;

                if (isAllGroups && isAllStaff)
                {
                    objMbr = db.Members.ToList();
                    return Json(objMbr, JsonRequestBehavior.AllowGet);
                }
                else if (!isAllGroups && isAllStaff)
                {
                    var data = from c in db.Members
                               where c.GroupCode == sGrpCode
                               select c;
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                }
                else if (isAllGroups && !isAllStaff)
                {
                    var data = from c in db.Members
                               where c.StaffId == iStaffId
                               select c;
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from c in db.Members
                               where c.StaffId == iStaffId && c.GroupCode == sGrpCode
                               select c;
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                }

                
            }
            catch (Exception ex)
            {
                logger.Error("Error in MemberList() Get Method" + ex.InnerException);
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
                logger.Error("Error in GetMember() Get Method" + ex.InnerException);
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

                    objMbr = db.Members.Where(c => c.MbrId == iMbrID).FirstOrDefault();


                    db.Members.Remove(objMbr);
                    db.SaveChanges();
                    logger.Info(Session["UserID"] + ": removed this member " + objMbr.MbrName + DateTime.Now);
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in RemoveMember() Get Method" + ex.InnerException);
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult EditMember(int iMbrID)
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MemberViewModel objMbr = new MemberViewModel();

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
                                                            Value = p.GroupCode
                                                        }).ToList();
                    objMbr.StaffMbrList = StaffList;
                    objMbr.GrpNameList = GrpCodeList;
                    #endregion

                    if (iMbrID > 0)
                    {
                        Member MbrObj = new Member();
                        FinGroup GrpID = new FinGroup();
                        Dictionary<int, string> myDic = new Dictionary<int, string>();

                        iMbrID = iMbrID == 0 ? Convert.ToInt32(Session["MbrID"]) : iMbrID;

                        if (iMbrID > 0)
                        {
                            MbrObj = db.Members
                                   .Where(p => p.MbrId == iMbrID)
                                   .Select(p => p).FirstOrDefault();

                            if (MbrObj == null)
                            {
                                // Member not found, return with error
                                objMbr.IsSucess = "NotFound";
                                return View(objMbr);
                            }

                            // Safe null check for AdharaCardNum
                            if (!string.IsNullOrEmpty(MbrObj.AdharaCardNum) && 
                                db.Members.Where(c => c.AdharaCardNum == MbrObj.AdharaCardNum.Trim()).ToList().Count > 0)
                            {
                                objMbr.IsSucess = "A";
                            }

                            //if (string.IsNullOrEmpty(objMbr.IsSucess))
                            //{
                                GrpID = db.FinGroups
                                      .Where(p => p.GroupCode == MbrObj.GroupCode)
                                      .Select(p => p).FirstOrDefault();

                                objMbr.MbrName = MbrObj.MbrName ?? "";
                                objMbr.MbrID = iMbrID;
                                objMbr.GrpCode = MbrObj.GroupCode ?? "";
                                objMbr.HsbndName = MbrObj.Husbandname ?? "";
                                objMbr.Age = MbrObj.Age ?? 0; // Safe null handling
                                objMbr.MbrStatus = MbrObj.MbrStatus ?? "";
                                objMbr.StaffID = MbrObj.StaffId ?? 0; // Safe null handling
                                objMbr.MbrAddress = MbrObj.MbrAddress ?? "";
                                objMbr.Gen = MbrObj.Gender ?? "";
                                objMbr.CantactNum = MbrObj.CantactNo ?? "";
                                objMbr.MbrDOJ = MbrObj.DOJ ?? "";
                                objMbr.Nominee = MbrObj.Nominee ?? "";
                                objMbr.PhoneNo2 = MbrObj.PhoneNo2 ?? "";
                                objMbr.AdharaCardNum = MbrObj.AdharaCardNum ?? "";
                                objMbr.RationCardNum = MbrObj.RCardNo ?? "";

                                // Safe date parsing for DOJ
                                try
                                {
                                    if (!string.IsNullOrEmpty(MbrObj.DOJ) && MbrObj.DOJ.Contains("/"))
                                    {
                                        string[] dateParts = MbrObj.DOJ.Split('/');
                                        if (dateParts.Length == 3)
                                        {
                                            objMbr.CrM = dateParts[0]; // Month
                                            objMbr.CrD = dateParts[1]; // Day
                                            objMbr.CrY = dateParts[2]; // Year
                                        }
                                        else
                                        {
                                            objMbr.CrD = "01";
                                            objMbr.CrM = "01";
                                            objMbr.CrY = "2000";
                                        }
                                    }
                                    else
                                    {
                                        objMbr.CrD = "01";
                                        objMbr.CrM = "01";
                                        objMbr.CrY = "2000";
                                    }
                                }
                                catch (Exception)
                                {
                                    objMbr.CrD = "01";
                                    objMbr.CrM = "01";
                                    objMbr.CrY = "2000";
                                }

                                // Don't override GrpCode again - it's already set above
                                // objMbr.GrpCode = MbrObj.GroupCode; // REMOVED - was causing issues
                            //}
                            logger.Info(Session["UserID"] + ": changed info of this member " + objMbr.MbrName + DateTime.Now);
                            objMbr.IsSucess = "0";
                        }

                        return View(objMbr);
                    }
                    
                    return View(objMbr);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditMember() Get Method" + ex.InnerException);
                // Create a new model with dropdown lists populated in case of error
                MemberViewModel errorModel = new MemberViewModel();
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
                                                        Value = p.GroupCode
                                                    }).ToList();
                errorModel.StaffMbrList = StaffList;
                errorModel.GrpNameList = GrpCodeList;
                return View(errorModel);
            }

        }

        [HttpPost]
        public ActionResult EditMember(MemberViewModel objMbr)
        {
            try
            {
                //if (!string.IsNullOrEmpty(Convert.ToString(Session["IsEditableMemberInfo"])) &&
                //                                      Convert.ToBoolean(Session["IsEditableMemberInfo"]) == true)
                //{

                if (objMbr != null)
                    {
                        MicroFinanceEntities db = new MicroFinanceEntities();
                        FinGroup Data = new FinGroup();
                        //int iGrpID = Convert.ToInt32(objMbr.GrpCode);
                        Data = db.FinGroups
                                         .Where(p => p.GroupCode == objMbr.GrpCode)
                                         .Select(p => p).FirstOrDefault();

                        Member objCrMbr = new Member();
                        objCrMbr.MbrName = objMbr.MbrName;
                        objCrMbr.GroupCode = Data.GroupCode;
                        objCrMbr.GrpName = Data.GrpName; // Add group name
                        objCrMbr.Husbandname = objMbr.HsbndName;
                        objCrMbr.Age = objMbr.Age;
                        objCrMbr.MbrStatus = objMbr.MbrStatus;
                        objCrMbr.Gender = objMbr.Gen;
                        objCrMbr.CantactNo = objMbr.CantactNum;
                        
                        // Handle DOJ properly - use MbrDOJ if available
                        if (!string.IsNullOrEmpty(objMbr.MbrDOJ))
                        {
                            objCrMbr.DOJ = objMbr.MbrDOJ;
                        }
                        else
                        {
                            objCrMbr.DOJ = Convert.ToString(objMbr.CrD + "/" + objMbr.CrM + "/" + objMbr.CrY);
                        }
                        
                        objCrMbr.WithdrawDt = Convert.ToString(DateTime.Now);
                        objCrMbr.StaffId = objMbr.StaffID;
                        
                        // Get staff name from StaffId
                        if (objMbr.StaffID > 0)
                        {
                            var staff = db.Staffs.FirstOrDefault(s => s.StaffID == objMbr.StaffID);
                            objCrMbr.StaffName = staff?.StaffName;
                        }
                        
                        objCrMbr.MbrAddress = objMbr.MbrAddress;
                        objCrMbr.RCardNo = objMbr.RationCardNum;
                        objCrMbr.AdharaCardNum = objMbr.AdharaCardNum; // Add Adhaar card number
                        objCrMbr.Nominee = objMbr.Nominee; // Add nominee
                        objCrMbr.PhoneNo2 = objMbr.PhoneNo2; // Add phone number 2
                        objCrMbr.MbrId = objMbr.MbrID;
                     
                        db.Entry(objCrMbr).State = EntityState.Modified;
                        db.SaveChanges();
                        objMbr.IsSucess = "1";
                        logger.Info(Session["UserID"] + ": changed info of this member " + objMbr.MbrName + DateTime.Now);
                        
                        // On successful update, redirect to member list page
                        TempData["SuccessMessage"] = "Member information updated successfully!";
                        return RedirectToAction("MemberDetails_New");
                    }

            }
            catch (Exception ex)
            {
                logger.Error("Error in EditMember() Post Method" + ex.InnerException);
                objMbr.IsSucess = "2";
                ModelState.AddModelError("", "Error updating member information. Please try again.");
            }
            
            // If we reach here, there was an error - repopulate dropdowns and stay on same page
            try
            {
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
                                                        Value = p.GroupCode
                                                    }).ToList();
                objMbr.StaffMbrList = StaffList;
                objMbr.GrpNameList = GrpCodeList;
            }
            catch (Exception ex)
            {
                logger.Error("Error populating dropdown lists in EditMember() Post Method: " + ex.Message);
                // Even if dropdown population fails, we still want to show the form with error
            }
            return View(objMbr);
        }

        [HttpGet]
        public string UploadFile(string sFilePath)
        {
            string sResult = string.Empty;
            sResult = "True";

            string sourceFile = @"C:\Temp\MaheshTX.txt";
            string destinationFile = @"C:\Temp\Data\MaheshTXCopied.txt";

            try
            {

              //  File.Copy(sFilePath, destinationFile, true);

            }

            catch (IOException iox)
            {

                Console.WriteLine(iox.Message);

            }           
            return sResult;            
        }

        // New action methods for modernized views
        [HttpGet]
        public ActionResult CreateMember_New()
        {
            return CreateMember();
        }

        [HttpPost]
        public ActionResult CreateMember_New(MemberViewModel objMbr)
        {
            // Check if this is an AJAX request
            if (Request.IsAjaxRequest())
            {
                try
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();

                    // Check for duplicate Aadhaar
                    if (!string.IsNullOrEmpty(objMbr.AdharaCardNum) && 
                        db.Members.Any(c => c.AdharaCardNum == objMbr.AdharaCardNum.Trim()))
                    {
                        return Json(new { success = false, message = "Member with this Aadhaar number already exists." });
                    }

                    // Get Group details
                    var objGrp = db.FinGroups.FirstOrDefault(c => c.GroupCode == objMbr.GrpCode);
                    if (objGrp == null)
                    {
                        return Json(new { success = false, message = "Selected group not found." });
                    }

                    // Get Staff name
                    int iStaffID = objMbr.StaffID;
                    string sStaffName = db.Staffs
                        .Where(p => p.StaffID == iStaffID)
                        .Select(p => p.StaffName)
                        .FirstOrDefault() ?? "";

                    // Create new member
                    var objCrMbr = new Member
                    {
                        MbrName = objMbr.MbrName,
                        GroupCode = objGrp.GroupCode,
                        GrpName = objGrp.GrpName,
                        Husbandname = objMbr.HsbndName ?? "",
                        Age = objMbr.Age,
                        MbrStatus = objMbr.MbrStatus ?? "Active",
                        Gender = objMbr.Gen,
                        CantactNo = objMbr.CantactNum ?? "",
                        DOJ = objMbr.MbrDOJ ?? DateTime.Now.ToString("dd/MM/yyyy"),
                        WithdrawDt = "",
                        StaffId = iStaffID,
                        StaffName = sStaffName,
                        AdharaCardNum = objMbr.AdharaCardNum?.Trim() ?? "",
                        Nominee = objMbr.Nominee ?? "",
                        PhoneNo2 = objMbr.PhoneNo2 ?? "",
                        MbrAddress = objMbr.MbrAddress ?? "",
                        RCardNo = objMbr.RationCardNum ?? ""
                    };

                    db.Members.Add(objCrMbr);
                    db.SaveChanges();

                    logger.Info(Session["UserID"] + ": Created member: " + objMbr.MbrName + " with ID: " + objCrMbr.MbrId + " at " + DateTime.Now);

                    return Json(new { success = true, message = "Member registered successfully!", memberId = objCrMbr.MbrId });
                }
                catch (Exception ex)
                {
                    logger.Error("Error in CreateMember_New() AJAX Post: " + ex.Message);
                    return Json(new { success = false, message = "Error creating member: " + ex.Message });
                }
            }
            
            // For non-AJAX requests, use the original method
            return CreateMember(objMbr);
        }

        [HttpGet]
        public ActionResult MemberDetails_New()
        {
            return MemberDetails();
        }

        [HttpGet]
        public ActionResult EditMember_New(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                return EditMember(id.Value);
            }
            else
            {
                return RedirectToAction("MemberDetails_New");
            }
        }

        [HttpPost]
        public ActionResult EditMember_New(MemberViewModel objMbr)
        {
            return EditMember(objMbr);
        }
    }
}
