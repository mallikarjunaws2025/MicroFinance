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
                    return RedirectToAction("Index_New", "Charts");
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
            // Set cache control headers to prevent caching of login page
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate, max-age=0");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            
            // Clear any existing session data
            if (Session["UserID"] != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            
            MicroFinanceEntities db = new MicroFinanceEntities();
            
            return View();
        }

        [HttpPost]
        public ActionResult StaffLogin(StaffSignIn objUser)
        {
            try
            {
                logger.Info("Logged in User Id : " + objUser.UserID + " at time" + DateTime.Now);
                logger.Info("Password received: " + (string.IsNullOrEmpty(objUser.Password) ? "NULL/EMPTY" : "HAS VALUE"));
                
                // Debug: Check form parameters directly
                string formUserID = Request.Form["UserID"];
                string formPassword = Request.Form["Password"];
                logger.Info("Form UserID: " + formUserID);
                logger.Info("Form Password: " + (string.IsNullOrEmpty(formPassword) ? "NULL/EMPTY" : "HAS VALUE"));
                
                // If model binding failed, use form values directly
                if (string.IsNullOrEmpty(objUser.Password) && !string.IsNullOrEmpty(formPassword))
                {
                    objUser.Password = formPassword;
                    logger.Info("Used form password instead of model password");
                }
                if (string.IsNullOrEmpty(objUser.UserID) && !string.IsNullOrEmpty(formUserID))
                {
                    objUser.UserID = formUserID;
                    logger.Info("Used form UserID instead of model UserID");
                }
                
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
                        
                        return RedirectToAction("Index_New", "Charts");
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

        [HttpGet]
        public ActionResult CreateBranch_New()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                BranchViewModel objBranchViewModel = new BranchViewModel();
                objBranchViewModel.IsSucess = "0";
                return View(objBranchViewModel);
            }
            else
            {
                return RedirectToAction("StaffLogin_New", "Staff");
            }
        }

        [HttpPost]
        public ActionResult CreateBranch_New(BranchViewModel objBranchViewModel)
        {
            try
            {
                logger.Info(Session["UserID"] + " this user created new Branch at the time of : " + DateTime.Now);
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
                logger.Error("Error occured in CreateBranch_New() Post method" + objEx.InnerException.ToString());
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
            Session["UsrType"] = null;
            ViewBag.UsrType = null;
            Session["UserID"] = null;
            Session["ValideUsr"] = null;
            Session["StaffID"] = null;
            Helper.bIsValidUser = false;
            ViewBag.IsAdmin = false;
            ViewBag.ValideUsr = "No";
            Helper.sGuid = string.Empty;
            
            // Completely abandon the session
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            
            // Set comprehensive cache control headers
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetNoServerCaching();
            
            // Additional cache control headers
            Response.Headers.Remove("Cache-Control");
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate, max-age=0");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            
            // Create a redirect result with cache busting
            var redirectUrl = Url.Action("StaffLogin", "Staff") + "?t=" + DateTime.Now.Ticks;
            return Redirect(redirectUrl);
        }



        [HttpGet]
        public ActionResult StaffList()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    // Redirect to the new modernized staff list
                    return RedirectToAction("StaffList_New", "Staff");
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
            // Redirect to the new modern branch list view
            return RedirectToAction("BranchList_New");
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
        [HttpGet]
        public ActionResult CreateStaff_New()
        {
            // Reuse the existing CreateStaff logic but return the new view
            var result = CreateStaff();
            if (result is ViewResult viewResult)
            {
                viewResult.ViewName = "CreateStaff_New";
            }
            return result;
        }
        
        [HttpPost]
        public ActionResult CreateStaff_New(NewStaff objStaff)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    logger.Info(Session["UserID"] + " creating new staff member at " + DateTime.Now);
                    
                    using (var db = new MicroFinanceEntities())
                    {
                        // Create Staff record
                        Staff objNewStaff = new Staff();
                        objNewStaff.StaffName = objStaff.StaffName;
                        objNewStaff.DOB = objStaff.DOB;
                        objNewStaff.DOJ = objStaff.DOJ;
                        objNewStaff.Status = objStaff.Status;

                        db.Staffs.Add(objNewStaff);
                        db.SaveChanges();

                        // Create StaffLogin record if RolePermission is provided
                        if (!string.IsNullOrEmpty(objStaff.RolePermission))
                        {
                            TestApp.DB.StaffLogin objStaffLogin = new TestApp.DB.StaffLogin();
                            objStaffLogin.StaffId = objNewStaff.StaffID;
                            objStaffLogin.UserName = objStaff.StaffName; // Default username as staff name
                            objStaffLogin.Password = "password123"; // Default password (should be changed)
                            objStaffLogin.RolePermission = objStaff.RolePermission;
                            objStaffLogin.IsLocked = 0;

                            db.StaffLogins.Add(objStaffLogin);
                            db.SaveChanges();
                        }

                        objStaff.IsSucess = "1";
                        logger.Info(Session["UserID"] + " created staff " + objStaff.StaffName + " at " + DateTime.Now);
                        return View(objStaff);
                    }
                }
                else
                {
                    objStaff.IsSucess = "";
                    return View(objStaff);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in CreateStaff_New: " + ex.Message);
                objStaff.IsSucess = "2";
                return View(objStaff);
            }
        }

        [HttpGet]
        public ActionResult StaffList_New()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                
                // Create a combined view model with staff and their roles
                var staffWithRoles = (from staff in db.Staffs
                                    join login in db.StaffLogins on staff.StaffID equals login.StaffId into staffLogins
                                    from login in staffLogins.DefaultIfEmpty()
                                    select new TestApp.VModels.StaffWithRoleViewModel
                                    {
                                        StaffID = staff.StaffID,
                                        StaffName = staff.StaffName,
                                        DOB = staff.DOB,
                                        DOJ = staff.DOJ,
                                        Status = staff.Status,
                                        RolePermission = login != null ? login.RolePermission : "Not Assigned",
                                        HasLogin = login != null,
                                        ContactNum = "" // This field is not stored in DB
                                    }).ToList();
                
                return View(staffWithRoles);
            }
            else
            {
                return RedirectToAction("StaffLogin_New", "Staff");
            }
        }

        [HttpPost]
        public JsonResult DeleteStaff(int id)
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    var staff = db.Staffs.Find(id);
                    if (staff != null)
                    {
                        db.Staffs.Remove(staff);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Staff deleted successfully" });
                    }
                    return Json(new { success = false, message = "Staff not found" });
                }
                return Json(new { success = false, message = "Unauthorized access" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error deleting staff");
                return Json(new { success = false, message = "An error occurred while deleting staff" });
            }
        }

        public ActionResult BranchList_New()
        {
            try
            {
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                    MicroFinanceEntities db = new MicroFinanceEntities();
                    var branches = db.Branches.ToList();
                    return View(branches);
                }
                return RedirectToAction("StaffLogin_New");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading branch list");
                return View(new List<Branch>());
            }
        }
    }
}
