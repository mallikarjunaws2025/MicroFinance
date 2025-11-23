using TestApp.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
                
                // Populate staff list for the dropdown
                MicroFinanceEntities db = new MicroFinanceEntities();
                List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                  where p.Status == "Active" || string.IsNullOrEmpty(p.Status)
                                                  select new SelectListItem
                                                  {
                                                      Text = p.StaffName + " (ID: " + p.StaffID + ")",
                                                      Value = p.StaffID.ToString()
                                                  }).ToList();
                
                // Add default option
                StaffList.Insert(0, new SelectListItem { Text = "-- Select Manager --", Value = "" });
                objBranchViewModel.StaffList = StaffList;
                
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
            MicroFinanceEntities db = new MicroFinanceEntities();

            try
            {
                // Always populate staff list in case we need to return to the view
                List<SelectListItem> StaffList = (from p in db.Staffs.AsEnumerable()
                                                  where p.Status == "Active" || string.IsNullOrEmpty(p.Status)
                                                  select new SelectListItem
                                                  {
                                                      Text = p.StaffName + " (ID: " + p.StaffID + ")",
                                                      Value = p.StaffID.ToString()
                                                  }).ToList();

                StaffList.Insert(0, new SelectListItem { Text = "-- Select Manager --", Value = "" });
                objBranchViewModel.StaffList = StaffList;

                if (ModelState.IsValid)
                {
                    logger.Info(Session["UserID"] + " this user created new Branch at the time of : " + DateTime.Now);

                    Branch objCrBranch = new Branch();
                    objCrBranch.BranchCode = objBranchViewModel.BranchCode;
                    objCrBranch.BranchName = objBranchViewModel.BranchName;
                    objCrBranch.BAddress = objBranchViewModel.BranchAddress;
                    objCrBranch.City = objBranchViewModel.City;
                    objCrBranch.State = objBranchViewModel.State;
                    objCrBranch.OpenDate = objBranchViewModel.OpenDateday + "/" + objBranchViewModel.OpenDateMonth + "/" + objBranchViewModel.OpenDateYear;
                    objCrBranch.PinCode = objBranchViewModel.PinCode;

                    // Map StaffId to ManagerID - handle case where no manager is selected
                    objCrBranch.ManagerID = objBranchViewModel.StaffId > 0 ? objBranchViewModel.StaffId : (int?)null;

                    db.Branches.Add(objCrBranch);
                    db.SaveChanges();

                    objBranchViewModel.IsSucess = "1";
                    logger.Info("Branch created successfully: " + objBranchViewModel.BranchName);
                    return View(objBranchViewModel);
                }
                else
                {
                    objBranchViewModel.IsSucess = "0";
                    return View(objBranchViewModel);
                }
            }
            catch (Exception objEx)
            {
                logger.Error("Error occurred in CreateBranch_New() Post method: " + objEx.Message);
                if (objEx.InnerException != null)
                {
                    logger.Error("Inner exception: " + objEx.InnerException.Message);
                }

                ModelState.AddModelError(string.Empty, "Error while creating branch. Please try again.");
                objBranchViewModel.IsSucess = "2";
                return View(objBranchViewModel);
            }
            finally
            {
                db?.Dispose();
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
            MicroFinanceEntities db = null;
            try
            {
                if (string.IsNullOrEmpty(UserModel))
                {
                    logger.Error("UserModel is null or empty in EditStaff method");
                    return Json("No", JsonRequestBehavior.AllowGet);
                }

                db = new MicroFinanceEntities();
                
                // Parse the UserModel string with validation
                var parts = UserModel.Split(',');
                if (parts.Length < 5)
                {
                    logger.Error("Invalid UserModel format. Expected 5 parts but got " + parts.Length);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Validate and parse staff ID
                if (!int.TryParse(parts[0], out int staffID))
                {
                    logger.Error("Invalid staff ID format: " + parts[0]);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Fetch the existing staff from database
                Staff objNewStaff = db.Staffs.Find(staffID);
                
                if (objNewStaff == null)
                {
                    logger.Error("Staff not found with ID: " + staffID);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Validate input data before updating
                string staffName = parts[1]?.Trim();
                string dob = parts[2]?.Trim();
                string doj = parts[3]?.Trim();
                string status = parts[4]?.Trim();
                
                // Enhanced validation
                if (string.IsNullOrEmpty(staffName))
                {
                    logger.Error("Staff name cannot be empty");
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Check string length limits to prevent validation errors
                if (staffName.Length > 50) // Assuming max length based on typical database constraints
                {
                    logger.Error("Staff name too long: " + staffName.Length + " characters");
                    return Json("Error: Staff name is too long (max 50 characters)", JsonRequestBehavior.AllowGet);
                }
                
                if (!string.IsNullOrEmpty(dob) && dob.Length > 20)
                {
                    logger.Error("DOB field too long: " + dob.Length + " characters");
                    return Json("Error: Date of birth format is too long", JsonRequestBehavior.AllowGet);
                }
                
                if (!string.IsNullOrEmpty(doj) && doj.Length > 20)
                {
                    logger.Error("DOJ field too long: " + doj.Length + " characters");
                    return Json("Error: Date of joining format is too long", JsonRequestBehavior.AllowGet);
                }
                
                if (!string.IsNullOrEmpty(status) && status.Length > 20)
                {
                    logger.Error("Status field too long: " + status.Length + " characters");
                    return Json("Error: Status value is too long", JsonRequestBehavior.AllowGet);
                }
                
                // Update the properties only if they're different to avoid unnecessary updates
                if (objNewStaff.StaffName != staffName)
                    objNewStaff.StaffName = staffName;
                    
                if (objNewStaff.DOB != dob)
                    objNewStaff.DOB = dob;
                    
                if (objNewStaff.DOJ != doj)
                    objNewStaff.DOJ = doj;
                    
                if (objNewStaff.Status != status)
                    objNewStaff.Status = status;

                // Validate the entity before saving
                var validationErrors = db.Entry(objNewStaff).GetValidationResult();
                if (!validationErrors.IsValid)
                {
                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        logger.Error($"Validation Error - Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                    return Json("Validation Error: Invalid data provided", JsonRequestBehavior.AllowGet);
                }

                // Save changes with better error handling
                db.Entry(objNewStaff).State = EntityState.Modified;
                int changesCount = db.SaveChanges();
                
                if (changesCount > 0)
                {
                    logger.Info(Session["UserID"] + " changed " + objNewStaff.StaffName + " staff details at " + DateTime.Now);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    logger.Info("No changes were made to staff ID: " + staffID);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException dbValEx)
            {
                logger.Error("Entity validation error in EditStaff(): " + dbValEx.Message);
                
                // Log detailed validation errors
                foreach (var validationErrors in dbValEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        logger.Error(string.Format("Property: {0} Error: {1}", 
                            validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                
                return Json("Validation Error: Please check the data format and try again", JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException dbEx)
            {
                logger.Error("Database update error in EditStaff(): " + dbEx.Message);
                if (dbEx.InnerException != null)
                {
                    logger.Error("Database inner exception: " + dbEx.InnerException.Message);
                }
                return Json("Database Error: Unable to save staff changes", JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException invEx)
            {
                logger.Error("Invalid operation error in EditStaff(): " + invEx.Message);
                return Json("Operation Error: " + invEx.Message, JsonRequestBehavior.AllowGet);
            }
            catch (FormatException fEx)
            {
                logger.Error("Format error in EditStaff(): " + fEx.Message);
                return Json("Format Error: Invalid data format", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in EditStaff() method: " + ex.Message);
                if (ex.InnerException != null)
                {
                    logger.Error("Inner exception: " + ex.InnerException.Message);
                }
                return Json("Error: Unable to update staff details", JsonRequestBehavior.AllowGet);
            }
            finally
            {
                // Ensure proper disposal of DbContext
                db?.Dispose();
            }
        }

        [HttpGet]
        public JsonResult EditBranch(string UserModel)
        {
            MicroFinanceEntities db = null;
            try
            {
                if (string.IsNullOrEmpty(UserModel))
                {
                    logger.Error("UserModel is null or empty in EditBranch method");
                    return Json("No", JsonRequestBehavior.AllowGet);
                }

                db = new MicroFinanceEntities();
                
                // Parse the UserModel string with validation
                var parts = UserModel.Split(',');
                if (parts.Length < 9)
                {
                    logger.Error("Invalid UserModel format. Expected 9 parts but got " + parts.Length);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Validate and parse branch ID
                if (!int.TryParse(parts[0], out int branchID))
                {
                    logger.Error("Invalid branch ID format: " + parts[0]);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Fetch the existing branch from database
                Branch objNewBranch = db.Branches.Find(branchID);
                
                if (objNewBranch == null)
                {
                    logger.Error("Branch not found with ID: " + branchID);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Validate input data before updating
                string branchCode = parts[1]?.Trim();
                string branchName = parts[2]?.Trim();
                string openDate = parts[3]?.Trim();
                string address = parts[4]?.Trim();
                string city = parts[5]?.Trim();
                string state = parts[6]?.Trim();
                
                if (string.IsNullOrEmpty(branchCode))
                {
                    logger.Error("Branch code cannot be empty");
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                if (string.IsNullOrEmpty(branchName))
                {
                    logger.Error("Branch name cannot be empty");
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                // Validate and parse numeric fields
                if (!int.TryParse(parts[7], out int pinCode))
                {
                    logger.Error("Invalid pin code format: " + parts[7]);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }
                
                if (!int.TryParse(parts[8], out int managerID))
                {
                    logger.Error("Invalid manager ID format: " + parts[8]);
                    return Json("No", JsonRequestBehavior.AllowGet);
                }

                // Update the properties only if they're different
                if (objNewBranch.BranchCode != branchCode)
                    objNewBranch.BranchCode = branchCode;
                    
                if (objNewBranch.BranchName != branchName)
                    objNewBranch.BranchName = branchName;
                    
                if (objNewBranch.OpenDate != openDate)
                    objNewBranch.OpenDate = openDate;
                    
                if (objNewBranch.BAddress != address)
                    objNewBranch.BAddress = address;
                    
                if (objNewBranch.City != city)
                    objNewBranch.City = city;
                    
                if (objNewBranch.State != state)
                    objNewBranch.State = state;
                    
                if (objNewBranch.PinCode != pinCode)
                    objNewBranch.PinCode = pinCode;
                    
                if (objNewBranch.ManagerID != managerID)
                    objNewBranch.ManagerID = managerID;

                // Validate the entity before saving
                var validationErrors = db.Entry(objNewBranch).GetValidationResult();
                if (!validationErrors.IsValid)
                {
                    foreach (var error in validationErrors.ValidationErrors)
                    {
                        logger.Error($"Validation Error - Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                    return Json("Validation Error: Invalid data provided", JsonRequestBehavior.AllowGet);
                }

                // Save changes
                int changesCount = db.SaveChanges();
                
                if (changesCount > 0)
                {
                    logger.Info(Session["UserID"] + " changed " + objNewBranch.BranchName + " branch details at " + DateTime.Now);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    logger.Info("No changes were made to branch ID: " + branchID);
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException dbValEx)
            {
                logger.Error("Entity validation error in EditBranch(): " + dbValEx.Message);
                
                foreach (var validationErrors in dbValEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        logger.Error(string.Format("Property: {0} Error: {1}", 
                            validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                
                return Json("Validation Error: Please check the data format and try again", JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException dbEx)
            {
                logger.Error("Database update error in EditBranch(): " + dbEx.Message);
                if (dbEx.InnerException != null)
                {
                    logger.Error("Database inner exception: " + dbEx.InnerException.Message);
                }
                return Json("Database Error: Unable to save branch changes", JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException invEx)
            {
                logger.Error("Invalid operation error in EditBranch(): " + invEx.Message);
                return Json("Operation Error: " + invEx.Message, JsonRequestBehavior.AllowGet);
            }
            catch (FormatException fEx)
            {
                logger.Error("Format error in EditBranch(): " + fEx.Message);
                return Json("Format Error: Invalid data format", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in EditBranch() method: " + ex.Message);
                if (ex.InnerException != null)
                {
                    logger.Error("Inner exception: " + ex.InnerException.Message);
                }
                return Json("Error: Unable to update branch details", JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db?.Dispose();
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
                    
                    // Get branches with manager information
                    var branchViewModels = (from branch in db.Branches
                                          join staff in db.Staffs on branch.ManagerID equals staff.StaffID into staffGroup
                                          from manager in staffGroup.DefaultIfEmpty()
                                          select new BranchWithManagerViewModel
                                          {
                                              BranchID = branch.BranchID,
                                              BranchCode = branch.BranchCode,
                                              BranchName = branch.BranchName,
                                              OpenDate = branch.OpenDate,
                                              BAddress = branch.BAddress,
                                              City = branch.City,
                                              State = branch.State,
                                              PinCode = branch.PinCode,
                                              ManagerID = branch.ManagerID,
                                              ManagerName = manager != null ? manager.StaffName : null
                                          }).ToList();
                    
                    return View(branchViewModels);
                }
                return RedirectToAction("StaffLogin_New");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading branch list");
                return View(new List<BranchWithManagerViewModel>());
            }
        }

        [HttpGet]
        public JsonResult GetStaffList()
        {
            try
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                var staffList = (from p in db.Staffs.AsEnumerable()
                                where p.Status == "Active" || string.IsNullOrEmpty(p.Status)
                                select new SelectListItem
                                {
                                    Text = p.StaffName + " (ID: " + p.StaffID + ")",
                                    Value = p.StaffID.ToString()
                                }).ToList();

                return Json(staffList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error loading staff list: " + ex.Message);
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBranch(int id)
        {
            try
            {
                MicroFinanceEntities db = new MicroFinanceEntities();
                var branch = db.Branches.FirstOrDefault(b => b.BranchID == id);
                
                if (branch == null)
                {
                    return Json(new { success = false, message = "Branch not found" }, JsonRequestBehavior.AllowGet);
                }
                
                var branchData = new
                {
                    success = true,
                    BranchID = branch.BranchID,
                    BranchCode = branch.BranchCode ?? "",
                    BranchName = branch.BranchName ?? "",
                    OpenDate = branch.OpenDate ?? "",
                    BAddress = branch.BAddress ?? "",
                    City = branch.City ?? "",
                    State = branch.State ?? "",
                    PinCode = branch.PinCode ?? 0,
                    ManagerID = branch.ManagerID ?? 0
                };
                
                return Json(branchData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error loading branch details: " + ex.Message);
                return Json(new { success = false, message = "Error loading branch details" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
