using ManageFinancery.Data;
using ManageFinancery.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TestApp.Comman;
using TestApp.VModels;
using TestApp.VModels.Management;

namespace TestApp.Controllers
{
    /// <summary>
    /// Model First version of Staff Controller
    /// This controller uses the new MicroFinanceContext instead of MicroFinanceEntities
    /// </summary>
    public class StaffModelFirstController : Controller
    {
        public StaffModelFirstController()
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
                    using (var context = new MicroFinanceContext())
                    {
                        // Create new staff using Model First entities
                        var objNewStaff = new Staff();
                        objNewStaff.StaffName = objStaff.StaffName;
                        objNewStaff.FName = objStaff.FName;
                        objNewStaff.Address = objStaff.Address;
                        objNewStaff.City = objStaff.City;
                        objNewStaff.State = objStaff.State;
                        objNewStaff.PinCode = objStaff.PinCode;
                        objNewStaff.ContactNumber = objStaff.ContactNumber;
                        objNewStaff.DateOfBirth = objStaff.DateOfBirth;
                        objNewStaff.DateOfJoining = objStaff.DateOfJoining;
                        objNewStaff.Email = objStaff.Email;
                        objNewStaff.AadharCardNumber = objStaff.AadharCardNumber;
                        objNewStaff.PanCardNumber = objStaff.PanCardNumber;
                        objNewStaff.BranchId = objStaff.BranchId;
                        objNewStaff.Salary = objStaff.Salary;
                        objNewStaff.IsActive = true;

                        context.Staffs.Add(objNewStaff);
                        context.SaveChanges();

                        objStaff.IsSucess = "Staff Created Successfully";
                        ModelState.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CreateStaff Error: " + ex.Message);
                objStaff.IsSucess = "Error: " + ex.Message;
            }

            return View(objStaff);
        }

        [HttpPost]
        public ActionResult EditStaff(EditStaff objEditStaff)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    using (var context = new MicroFinanceContext())
                    {
                        // Find existing staff member - Model First approach
                        var staff = context.Staffs.Find(objEditStaff.StaffId);
                        
                        if (staff != null)
                        {
                            // Update staff properties
                            staff.StaffName = objEditStaff.StaffName;
                            staff.FName = objEditStaff.FName;
                            staff.Address = objEditStaff.Address;
                            staff.City = objEditStaff.City;
                            staff.State = objEditStaff.State;
                            staff.PinCode = objEditStaff.PinCode;
                            staff.ContactNumber = objEditStaff.ContactNumber;
                            staff.DateOfBirth = objEditStaff.DateOfBirth;
                            staff.DateOfJoining = objEditStaff.DateOfJoining;
                            staff.Email = objEditStaff.Email;
                            staff.AadharCardNumber = objEditStaff.AadharCardNumber;
                            staff.PanCardNumber = objEditStaff.PanCardNumber;
                            staff.BranchId = objEditStaff.BranchId;
                            staff.Salary = objEditStaff.Salary;

                            // Save changes - this should work without the previous SaveChanges error
                            context.SaveChanges();

                            objEditStaff.IsSucess = "Staff Updated Successfully";
                        }
                        else
                        {
                            objEditStaff.IsSucess = "Staff not found";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("EditStaff Error: " + ex.Message);
                objEditStaff.IsSucess = "Error: " + ex.Message;
            }

            return Json(objEditStaff, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStaffList()
        {
            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
            {
                try
                {
                    using (var context = new MicroFinanceContext())
                    {
                        var staffList = context.Staffs
                            .Where(s => s.IsActive == true)
                            .Select(s => new
                            {
                                s.StaffId,
                                s.StaffName,
                                s.FName,
                                s.ContactNumber,
                                s.Email,
                                s.DateOfJoining,
                                BranchName = s.Branch.BranchName
                            })
                            .ToList();

                        return Json(staffList, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("GetStaffList Error: " + ex.Message);
                    return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
    }
}