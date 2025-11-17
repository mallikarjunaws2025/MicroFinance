using TestApp.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestApp.Comman;
using TestApp.VModels;
using TestApp.VModels.Management;

namespace TestApp.Controllers
{
    public class StaffController : Controller
    {
        public StaffController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }

        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        [HttpGet]
        public ActionResult CreateStaff()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                NewStaff objStaff = new NewStaff();
                objStaff.IsSucess = "";
                return View(objStaff);
            }
            else
            {
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        [HttpPost]
        public ActionResult CreateStaff(NewStaff objStaff)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    Staff objNewStaff = new Staff();
                    objNewStaff.StaffName = objStaff.StaffName;
                    objNewStaff.DOB = objStaff.DOB; //objStaff.DOBD + "/" + objStaff.DOBM + "/" + objStaff.DOBY;
                    objNewStaff.DOJ = objStaff.DOJ;//objStaff.DOJD + "/" + objStaff.DOJM + "/" + objStaff.DOJY;
                    objNewStaff.Status = objStaff.Status;

                    db.Staffs.Add(objNewStaff);
                    db.SaveChanges();
                    objStaff.IsSucess = "1";
                    logger.Info(Session["UserID"] + " created this" + objStaff.StaffName + "staff account at the time" + DateTime.Now);
                    return View(objStaff);
                }
                else
                {
                    objStaff.IsSucess = "";
                    return View(objStaff);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in Create Staff Post Method()" + ex.InnerException);
                objStaff.IsSucess = "2";
                return View(objStaff);
            }
        }
        [HttpGet]
        public ActionResult StaffSignUp()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    StaffSignUP objStaffSignUP = new StaffSignUP();

                    List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                      select new SelectListItem
                                                      {
                                                          Text = p.StaffName,
                                                          Value = p.StaffID.ToString()
                                                      }).ToList();

                    objStaffSignUP.StaffList = StaffList;
                    return View(objStaffSignUP);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in StaffSignUp ()" + ex.InnerException);
            }
            return View();

        }
        [HttpGet]
        public ActionResult NewUserLoginAcc()
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) == "Admin")
                {
                    if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                    {
                        MicroFinanceEntities db = new MicroFinanceEntities();
                        StaffSignUP objStaffSignUP = new StaffSignUP();

                        List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                          select new SelectListItem
                                                          {
                                                              Text = p.StaffName,
                                                              Value = p.StaffID.ToString()
                                                          }).ToList();

                        objStaffSignUP.StaffList = StaffList;
                        return View(objStaffSignUP);
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
                logger.Error("Error in StaffSignUp ()" + ex.InnerException);
            }
            return View();

        }

        [HttpPost]
        public ActionResult NewUserLoginAcc(StaffSignUP objStaff)
        {
            try
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                  select new SelectListItem
                                                  {
                                                      Text = p.StaffName,
                                                      Value = p.StaffID.ToString()
                                                  }).ToList();

                objStaff.StaffList = StaffList;

                if (objStaff.Password == objStaff.ConfirmPassword)
                {

                    TestApp.DB.StaffLogin objStaffLogin = new TestApp.DB.StaffLogin();

                    objStaffLogin.StaffId = Convert.ToInt32(objStaff.StaffID);
                    objStaffLogin.UserName = objStaff.UserID;
                    objStaffLogin.Password = Helper.HashPassword(objStaff.Password);
                    objStaffLogin.RolePermission = objStaff.RolePermission;
                    objStaffLogin.IsLocked = 0;

                    db.StaffLogins.Add(objStaffLogin);
                    db.SaveChanges();
                    objStaff.IsSucess = "1";
                    logger.Info("Logged out User Id : " + objStaff.UserID + " at time" + DateTime.Now);
                    return View(objStaff);
                }
                else
                {
                    return View(objStaff);
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in StaffSignUp(): " + objEx.InnerException.ToString());
                ModelState.AddModelError(string.Empty, "Failed to create user login account");
                objStaff.IsSucess = "2";
                return View(objStaff);
            }
        }


        [HttpPost]
        public ActionResult StaffSignUp(StaffSignUP objStaff)
        {
            try
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                  select new SelectListItem
                                                  {
                                                      Text = p.StaffName,
                                                      Value = p.StaffID.ToString()
                                                  }).ToList();

                objStaff.StaffList = StaffList;

                if (objStaff.Password == objStaff.ConfirmPassword)
                {

                    TestApp.DB.StaffLogin objStaffLogin = new TestApp.DB.StaffLogin();

                    objStaffLogin.StaffId = Convert.ToInt32(objStaff.StaffID);
                    objStaffLogin.UserName = objStaff.UserID;
                    objStaffLogin.Password = Helper.HashPassword(objStaff.Password);
                    objStaffLogin.RolePermission = objStaff.RolePermission;
                    objStaffLogin.IsLocked = 0;

                    db.StaffLogins.Add(objStaffLogin);
                    db.SaveChanges();
                    objStaff.IsSucess = "1";
                    logger.Info("Logged out User Id : " + objStaff.UserID + " at time" + DateTime.Now);
                    return View(objStaff);
                }
                else
                {
                    return View(objStaff);
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in StaffSignUp(): " + objEx.InnerException.ToString());
                ModelState.AddModelError(string.Empty, "Failed to create user login account");
                objStaff.IsSucess = "2";
                return View(objStaff);
            }
        }


        [HttpGet]
        public ActionResult StaffLogin()
        {
            MicroFinanceEntities db = new MicroFinanceEntities();
            
            return View();
        }

        [HttpPost]
        public ActionResult StaffLogin(StaffSignIn objUser)
        {
            try
            {
                logger.Info("Logged in User Id : " + objUser.UserID + " at time" + DateTime.Now);
                //string uID = Convert.ToString(System.Guid.NewGuid());
                MicroFinanceEntities db = new MicroFinanceEntities();
                var Data = db.StaffLogins
                           .Where(p => p.UserName == objUser.UserID)
                           .Select(p => p).ToList();
                logger.Info("Before validate");
                if (Data.Count > 0 && Data[0].IsLocked == 0)
                {
                    bool isPasswordValid;
                    
                    // Check if password is already hashed (new format) or plain text (legacy)
                    if (Data[0].Password.Contains("=")) // Base64 encoded hash
                    {
                        isPasswordValid = Helper.VerifyPassword(objUser.Password, Data[0].Password);
                    }
                    else
                    {
                        // Legacy plain text password - hash it and update
                        isPasswordValid = Data[0].Password == objUser.Password;
                        if (isPasswordValid)
                        {
                            Data[0].Password = Helper.HashPassword(objUser.Password);
                            db.SaveChanges();
                        }
                    }
                    
                    if (isPasswordValid && Data[0].UserName == objUser.UserID)
                    {
                        string userRole = Data[0].RolePermission?.Trim();
                        bool isAdmin = !string.IsNullOrEmpty(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                        
                        Helper.IsAdmin = isAdmin;
                        ViewBag.IsAdmin = isAdmin;
                        Session["UsrType"] = userRole;
                        ViewBag.UsrType = userRole;
                        Session["UserID"] = objUser.UserID;
                        Session["ValideUsr"] = true;
                        Session["StaffID"] = Data[0].StaffId;
                        Helper.bIsValidUser = true;
                        
                        // Log successful login
                        var userLog = new UserLog
                        {
                            LoginName = objUser.UserID,
                            LogInDateTime = DateTime.Now,
                            StaffName = Data[0].StaffId.ToString()
                        };
                        db.UserLogs.Add(userLog);
                        db.SaveChanges();
                        
                        return RedirectToAction("Index", "Charts");
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login credentials or account is locked.");
                return View();
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in StaffLogin(): " + objEx.InnerException.ToString());
                ModelState.AddModelError(string.Empty, "Failed to acess user login account");
                return View();
            }
        }

        public ActionResult HomePage()
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
        [HttpGet]
        public ActionResult ChangePWD()
        {
            //if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            //{
                ChangePWD objUserLogin = new ChangePWD();
                objUserLogin.IsSucess = "";
                return View(objUserLogin);
            //}
            //else
            //{
            //    return RedirectToAction("StaffLogin", "Staff");
            //}

        }
        [HttpPost]
        public ActionResult ChangePWD(ChangePWD objUserLogin)
        {
            try
            {
                if (objUserLogin.Password != objUserLogin.ConfirmPassword)
                {
                    return View();
                }
                else
                {
                    logger.Info(Session["UserID"] + "this user changed thier password at the time of : " + DateTime.Now);
                    MicroFinanceEntities db = new MicroFinanceEntities();

                    TestApp.DB.StaffLogin Data = db.StaffLogins
                             .Where(p => p.UserName == objUserLogin.UserID)
                             .Select(p => p).FirstOrDefault();

                    Data.Password = objUserLogin.Password;
                    db.Entry(Data).State = EntityState.Modified;
                    db.SaveChanges();
                    objUserLogin.IsSucess = "1";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in ChangePWD() Post method" + objEx.InnerException.ToString());
                objUserLogin.IsSucess = "2";
                ModelState.AddModelError(string.Empty, "Please verify user login information");
                return View(objUserLogin);
            }
        }

        [HttpGet]
        public ActionResult CreateBranch()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                BranchViewModel objBranchViewModel = new BranchViewModel();
                objBranchViewModel.IsSucess = "0";
                return View(objBranchViewModel);
            }
            else
            {
                return RedirectToAction("StaffLogin", "Staff");
            }
        }

        [HttpPost]
        public ActionResult CreateBranch(BranchViewModel objBranchViewModel)
        {
            try
            {
                logger.Info(Session["UserID"] + "this user created new Branch at the time of : " + DateTime.Now);
                MicroFinanceEntities db = new MicroFinanceEntities();
                Branch objCrBranch = new Branch();

                objCrBranch.BranchCode = objBranchViewModel.BranchCode;
                objCrBranch.BranchName = objBranchViewModel.BranchName;
                objCrBranch.BAddress = objBranchViewModel.BranchAddress;
                objCrBranch.City = objBranchViewModel.City;
                objCrBranch.State = objBranchViewModel.State;
                objCrBranch.OpenDate = objBranchViewModel.OpenDateday + "/" + objBranchViewModel.OpenDateMonth + "/" + objBranchViewModel.OpenDateYear;
                objCrBranch.PinCode = objBranchViewModel.PinCode;
                objCrBranch.ManagerID = objBranchViewModel.StaffId;
                db.Branches.Add(objCrBranch);
                db.SaveChanges();
                objBranchViewModel.IsSucess = "1";
                return View(objBranchViewModel);
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in CreateBranch() Post method" + objEx.InnerException.ToString());
                ModelState.AddModelError(string.Empty, "Error while creating branch");
                objBranchViewModel.IsSucess = "2";
                return View(objBranchViewModel);
            }

        }

        public ActionResult Logout()
        {
            try
            {
                logger.Info("User logged out at the time of : " + DateTime.Now);
                
                // Log the logout activity
                if (Session["UserID"] != null)
                {
                    using (var db = new MicroFinanceEntities())
                    {
                        var userLog = new UserLog
                        {
                            LoginName = Session["UserID"].ToString(),
                            LogOutDateTime = DateTime.Now,
                            StaffName = Session["StaffID"]?.ToString() ?? "Unknown"
                        };
                        db.UserLogs.Add(userLog);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error logging user logout: " + ex.Message);
            }
            
            // Clear all session data
            Helper.IsAdmin = false;
            Session["UsrType"] = string.Empty;
            ViewBag.UsrType = string.Empty;
            Session["UserID"] = string.Empty;
            Session["ValideUsr"] = string.Empty;
            Helper.bIsValidUser = false;
            ViewBag.IsAdmin = false;
            ViewBag.ValideUsr = "No";
            Helper.sGuid = string.Empty;
            
            return RedirectToAction("StaffLogin", "Staff");
        }



        [HttpGet]
        public ActionResult StaffList()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    List<Staff> objGrp = new List<Staff>();
                    objGrp = db.Staffs.ToList();

                    return View(objGrp);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occured in StaffList() Get method" + ex.InnerException.ToString());
            }
            return View();
        }

        [HttpGet]
        public ActionResult BranchList()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    List<Branch> objGrp = new List<Branch>();
                    objGrp = db.Branches.ToList();

                    return View(objGrp);
                }
                else
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occured in BranchList() Get method" + ex.InnerException.ToString());
            }
            return View();
        }

        [HttpGet]
        public JsonResult EditStaff(string UserModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserModel))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    Staff objNewStaff = new Staff();
                    objNewStaff.StaffID = Convert.ToInt32(UserModel.Split(',')[0]);
                    objNewStaff.StaffName = UserModel.Split(',')[1];
                    objNewStaff.DOB = UserModel.Split(',')[2];
                    objNewStaff.DOJ = UserModel.Split(',')[3];
                    objNewStaff.Status = UserModel.Split(',')[4];


                    db.Entry(objNewStaff).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info(Session["UserID"] + "this user changed " + objNewStaff.StaffName + " staff details at the time" + DateTime.Now);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occured in EditStaff() Get method" + ex.InnerException.ToString());
                return Json("No", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult EditBranch(string UserModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserModel))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    Branch objNewBranch = new Branch();

                    objNewBranch.BranchID = Convert.ToInt32(UserModel.Split(',')[0]);
                    objNewBranch.BranchCode = UserModel.Split(',')[1];
                    objNewBranch.BranchName = UserModel.Split(',')[2];
                    objNewBranch.OpenDate = UserModel.Split(',')[3];
                    objNewBranch.BAddress = UserModel.Split(',')[4];
                    objNewBranch.City = UserModel.Split(',')[5];
                    objNewBranch.State = UserModel.Split(',')[6];
                    objNewBranch.PinCode = Convert.ToInt32(UserModel.Split(',')[7]);
                    objNewBranch.ManagerID = Convert.ToInt32(UserModel.Split(',')[8]);

                    db.Entry(objNewBranch).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info(Session["UserID"] + "this user changed " + objNewBranch.BranchName + "branch details at the time" + DateTime.Now);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occured in EditStaff() Get method" + ex.InnerException.ToString());
                return Json("No", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ChangePWDFormLogin()
        {
            ChangePWD objUserLogin = new ChangePWD();
            objUserLogin.IsSucess = "";
            return View(objUserLogin);
        }

        [HttpPost]
        public ActionResult ChangePWDFormLogin(ChangePWD objUserLogin)
        {
            try
            {
                if (objUserLogin.Password != objUserLogin.ConfirmPassword)
                {
                    return View();
                }
                else
                {
                    logger.Info(Session["UserID"] + "this user changed thier password at the time of : " + DateTime.Now);
                    MicroFinanceEntities db = new MicroFinanceEntities();

                    TestApp.DB.StaffLogin Data = db.StaffLogins
                             .Where(p => p.UserName == objUserLogin.UserID)
                             .Select(p => p).FirstOrDefault();

                    Data.Password = objUserLogin.Password;
                    db.Entry(Data).State = EntityState.Modified;
                    db.SaveChanges();
                    objUserLogin.IsSucess = "1";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error occured in ChangePWD() Post method" + objEx.InnerException.ToString());
                objUserLogin.IsSucess = "2";
                ModelState.AddModelError(string.Empty, "Please verify user login information");
                return View(objUserLogin);
            }
        }
    }
}
