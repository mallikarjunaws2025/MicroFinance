using TestApp.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.Comman;
using TestApp.VModels.Management;

namespace TestApp.Controllers
{
    public class ManagementController : Controller
    {
        public ManagementController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        [HttpGet]
        public ActionResult NewPartner()
        {
            AddNewPartner objPartner = new AddNewPartner();
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) == "Admin")
            {
                logger.Info("Opened from this user : " + Session["UserID"]);
                if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
                {
                   
                    objPartner.IsSucess = "";
                    return View(objPartner);
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

        [HttpPost]
        public ActionResult NewPartner(AddNewPartner objPartner)
        {
            PartnerDetail objPartnerDetails = new PartnerDetail();
             MicroFinanceEntities db = new MicroFinanceEntities();
            try
            {
                objPartnerDetails.PrtnrName = objPartner.PrtnrName;
                objPartnerDetails.Address = objPartner.Address;
                objPartnerDetails.CatntactNum = objPartner.CantactNum;
                objPartnerDetails.City = objPartner.City;
                objPartnerDetails.Statte = objPartner.Statte;
                objPartnerDetails.ZIP = objPartner.ZIP;
                 objPartnerDetails.AdharaCardNum = objPartner.AdharaCardNum;
                 objPartnerDetails.CrDate = objPartner.DOJ; //objPartner.DOJD + "/" + objPartner.DOJM +"/"+ objPartner.DOJY;
                objPartnerDetails.InvestmentAmt = objPartnerDetails.InvestmentAmt;

                db.PartnerDetails.Add(objPartnerDetails);
                db.SaveChanges();
                logger.Info(Session["UserID"] + ": created new Partner : "+ DateTime.Now);       
                objPartner.IsSucess = "1";
            }
            catch (Exception ex)
            {
                logger.Error("Error occured in NewPartner() Post: " + ex.InnerException.ToString());
                objPartner.IsSucess = "2";
            }
            return View(objPartner);
        }


        //[HttpGet]
        //public ActionResult InvesterList()
        //{
        //    List<Management> objGrp = new List<Management>();
        //    MicroFinanceEntities db = new MicroFinanceEntities();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Convert.ToString(Session["UsrType"])) && Convert.ToString(Session["UsrType"]) == "Admin")
        //        {
        //            if (Helper.IsValidUser(Convert.ToString(Session["ValideUsr"])))
        //            {
        //                objGrp = db.Managements.ToList();
        //            }
        //            else
        //            {
        //                return RedirectToAction("StaffLogin", "Staff");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Charts");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Error occured in InvesterList() Get: " + ex.InnerException.ToString());
        //    }
        //    return View(objGrp);
        //}

        //                        [HttpGet]
        //public JsonResult EditInvester(string UserModel)
        //{
        //    try
        //    {
        //        MicroFinanceEntities db = new MicroFinanceEntities();
        //        Management objInvest = new Management();
        //        if (!string.IsNullOrEmpty(UserModel))
        //        {
        //            objInvest.InvestID = Convert.ToInt32(UserModel.Split(',')[0]);
        //            objInvest.PrtnrID = Convert.ToInt32(UserModel.Split(',')[1]);
        //            objInvest.InvesterName = UserModel.Split(',')[2]; ;
        //            objInvest.InvestmentAmt = UserModel.Split(',')[3];
        //            objInvest.WithdrawAmt = UserModel.Split(',')[4];
        //            objInvest.BalanceAmt = UserModel.Split(',')[5];
        //            objInvest.WithdrawDate = UserModel.Split(',')[6];
        //            objInvest.ProfitAmt = UserModel.Split(',')[7];
        //            objInvest.LossAmt = UserModel.Split(',')[8];
        //            objInvest.DueAmt = UserModel.Split(',')[9];
        //            objInvest.Notes = UserModel.Split(',')[10];
        //            objInvest.CrDate = UserModel.Split(',')[11];

        //            db.Entry(objInvest).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();

        //            return Json("Success", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json("No", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Error occured in EditInvester() Get: " + ex.InnerException.ToString());
        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

    }
}
