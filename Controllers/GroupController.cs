using TestApp.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.Comman;
using TestApp.VModels.Group;

namespace TestApp.Controllers
{
    public class GroupController : Controller
    {
        public GroupController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        [HttpGet]
        public ActionResult CreateGroup()
        {
            Groups objGrps = new Groups();
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
                    List<SelectListItem> BranchCodeList = (from p in db.Branches.AsEnumerable()
                                                           select new SelectListItem
                                                           {
                                                               Text = p.BranchCode,
                                                               Value = p.BranchID.ToString()
                                                           }).ToList();

                    objGrps.IsSucess = "0";
                    objGrps.StaffMbrList = StaffList;
                    objGrps.BranchCodeList = BranchCodeList;
                    return View(objGrps);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error in CreateGroup() Get" + ex.InnerException);
                ModelState.AddModelError(string.Empty, "Error while loading page");
                return View(objGrps);
            }
        }

        [HttpPost]
        public ActionResult CreateGroup(Groups objGrps)
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                  select new SelectListItem
                                                  {
                                                      Text = p.StaffName,
                                                      Value = p.StaffID.ToString()
                                                  }).ToList();
                List<SelectListItem> BranchCodeList = (from p in db.Branches.AsEnumerable()
                                                       select new SelectListItem
                                                       {
                                                           Text = p.BranchCode,
                                                           Value = p.BranchID.ToString()
                                                       }).ToList();

                objGrps.IsSucess = "0";
                objGrps.StaffMbrList = StaffList;
                objGrps.BranchCodeList = BranchCodeList;


                FinGroup objCrGrp = new FinGroup();


                objCrGrp.GrpName = objGrps.GroupName;
                objCrGrp.GroupCode = objGrps.GroupCode;
                objCrGrp.RecoveryDay = objGrps.RecoveryDay;
                objCrGrp.Village = objGrps.Village;
                objCrGrp.Net_Members = objGrps.TotalMember;
                objCrGrp.CrDt = objGrps.DOC;//objGrps.CrM + "/" + objGrps.CrD + "/" + objGrps.CrY;
                objCrGrp.DOC = string.Empty; // objGrps.CCrD + "/" + objGrps.CCrM + "/" + objGrps.CCrY;
                objCrGrp.Distance = objGrps.Distance;
                objCrGrp.GStatus = objGrps.Status;
                objCrGrp.StaffID = objGrps.StaffID;
                objCrGrp.FormedBy = objGrps.FormedBy;
                objCrGrp.MeetingPlaceAddress = objGrps.MeetingPlaceAddress;
                objCrGrp.GType = objGrps.GroupType;

                db.FinGroups.Add(objCrGrp);
                db.SaveChanges();

                string sGrpID = Convert.ToString(objCrGrp.GroupID);

                if (!string.IsNullOrEmpty(sGrpID))
                {
                    if (Convert.ToInt32(sGrpID) <= 9)
                        sGrpID = "0" + sGrpID;


                    if (objGrps.GroupType == "Weekly")
                    {
                        objCrGrp.GroupCode = "W" + sGrpID;
                    }
                    else 
                    {
                        objCrGrp.GroupCode = "D" + sGrpID;
                    }

                    db.Entry(objCrGrp).State = EntityState.Modified;
                    db.SaveChanges();

                    objGrps.IsSucess = "1";
                    logger.Info(Session["UserID"] + ": Created New Group " + objCrGrp.GrpName +" :" + DateTime.Now);
                }

                return View(objGrps);
            }
            catch (Exception ex)
            {
                logger.Error("Error in CreateGroup() Post" + ex.InnerException);
                if (Convert.ToString(ex.HResult) == "-2146233087")
                {
                    objGrps.IsSucess = "Please try with different Group Name";
                    objGrps.IsSucess = "3";
                }
                ModelState.AddModelError(string.Empty, "Error while creating group");
                objGrps.IsSucess = "2";
                return View(objGrps);
            }
        }


        [HttpPost]
        public ActionResult ErrorPage()
        {
            return Redirect(Request.UrlReferrer.ToString());
        }


        [HttpGet]
        public ActionResult GrpList()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    List<FinGroup> objGrp = new List<FinGroup>();
                    objGrp = db.FinGroups.ToList();

                    return View(objGrp);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in GrpList() Get" + ex.InnerException);
            }
            return View();
        }


        [HttpGet]
        public JsonResult EditGroup(string UserModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) != "Admin")
                {
                    return Json("RoleIssue", JsonRequestBehavior.AllowGet);
                }

                if (!string.IsNullOrEmpty(UserModel))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    FinGroup objCrGrp = new FinGroup();

                    objCrGrp.GroupID = Convert.ToInt32(UserModel.Split(',')[0]);
                    objCrGrp.GroupCode = UserModel.Split(',')[1];
                    objCrGrp.BranchCode = UserModel.Split(',')[2];
                    objCrGrp.Village = UserModel.Split(',')[3];
                    objCrGrp.Net_Members = Convert.ToInt32(UserModel.Split(',')[4]);
                   
                    objCrGrp.DOC = UserModel.Split(',')[5];
                    objCrGrp.Distance = Convert.ToInt32(UserModel.Split(',')[6]);
                    objCrGrp.GStatus = UserModel.Split(',')[7];
                    objCrGrp.StaffID = Convert.ToInt32(UserModel.Split(',')[8]);
                    objCrGrp.FormedBy = UserModel.Split(',')[9];
                    objCrGrp.MeetingPlaceAddress = UserModel.Split(',')[10];
                    objCrGrp.GType = UserModel.Split(',')[11];
                    objCrGrp.GrpName = UserModel.Split(',')[12];

                    db.Entry(objCrGrp).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error in EditGroup() Get" + ex.InnerException);
                return Json("No", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult CreateGroup_New()
        {
            return CreateGroup();
        }
        
        [HttpPost]
        public ActionResult CreateGroup_New(Groups objGrps)
        {
            var result = CreateGroup(objGrps);
            // If it's a view result, change the view name
            if (result is ViewResult viewResult)
            {
                viewResult.ViewName = "CreateGroup_New";
            }
            return result;
        }

        [HttpGet]
        public ActionResult GrpList_New()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    List<FinGroup> objGrp = new List<FinGroup>();
                    objGrp = db.FinGroups.ToList();

                    return View("GrpList_New", objGrp);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in GrpList_New() Get" + ex.InnerException);
            }
            return View("GrpList_New");
        }

        [HttpGet]
        public ActionResult EditGroup_New(int id)
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    FinGroup finGroup = db.FinGroups.FirstOrDefault(g => g.GroupID == id);
                    
                    if (finGroup == null)
                    {
                        return RedirectToAction("GrpList_New");
                    }
                    
                    // Convert FinGroup to Groups model for the view
                    Groups groupModel = new Groups
                    {
                        GroupCode = finGroup.GroupCode,
                        GroupName = finGroup.GrpName,
                        RecoveryDay = finGroup.RecoveryDay,
                        Village = finGroup.Village,
                        TotalMember = finGroup.Net_Members ?? 0,
                        DOC = finGroup.DOC,
                        Distance = finGroup.Distance ?? 0,
                        Status = finGroup.GStatus,
                        StaffID = finGroup.StaffID ?? 0,
                        FormedBy = finGroup.FormedBy,
                        MeetingPlaceAddress = finGroup.MeetingPlaceAddress,
                        GroupType = finGroup.GType
                    };
                    
                    ViewBag.GroupID = finGroup.GroupID; // Pass the ID for updating
                    return View("EditGroup_New", groupModel);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditGroup_New() Get" + ex.InnerException);
                return RedirectToAction("GrpList_New");
            }
        }

        [HttpPost]
        public ActionResult EditGroup_New(Groups objGrp)
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    int groupId = Convert.ToInt32(Request.Form["GroupID"]);
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    FinGroup finGroup = db.FinGroups.FirstOrDefault(g => g.GroupID == groupId);
                    
                    if (finGroup != null)
                    {
                        // Update FinGroup with Groups model data
                        finGroup.GroupCode = objGrp.GroupCode;
                        finGroup.GrpName = objGrp.GroupName;
                        finGroup.RecoveryDay = objGrp.RecoveryDay;
                        finGroup.Village = objGrp.Village;
                        finGroup.Net_Members = objGrp.TotalMember;
                        finGroup.DOC = objGrp.DOC;
                        finGroup.Distance = objGrp.Distance;
                        finGroup.GStatus = objGrp.Status;
                        finGroup.StaffID = objGrp.StaffID;
                        finGroup.FormedBy = objGrp.FormedBy;
                        finGroup.MeetingPlaceAddress = objGrp.MeetingPlaceAddress;
                        finGroup.GType = objGrp.GroupType;
                        
                        db.Entry(finGroup).State = EntityState.Modified;
                        db.SaveChanges();
                        
                        TempData["Success"] = "Group updated successfully!";
                        return RedirectToAction("GrpList_New");
                    }
                    else
                    {
                        TempData["Error"] = "Group not found.";
                        return RedirectToAction("GrpList_New");
                    }
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in EditGroup_New() Post" + ex.InnerException);
                TempData["Error"] = "Error updating group. Please try again.";
                ViewBag.GroupID = Request.Form["GroupID"];
                return View("EditGroup_New", objGrp);
            }
        }
    }
}
