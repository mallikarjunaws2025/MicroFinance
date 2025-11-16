
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.ViewModels.ChartsData;
using TestApp.DB;
using TestApp.Comman;

namespace TestApp.Controllers
{
    public class ChartsController : Controller
    {
        public ChartsController()
        {
            ViewBag.IsAdmin = Helper.IsAdmin;
        }
        NLog.Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index()
        {
            try
            {
                string tempColName = string.Empty;
                string tempvals = string.Empty;

                MicroFinanceEntities db = new MicroFinanceEntities();
                var DueCllctedamt = db.spDueAndCollectedAmt().ToList();

                var sCounts = DueCllctedamt[0];// Convert.ToString(DueCllctedamt[0].CollectedAmount) + Convert.ToString("," + DueCllctedamt[0].Total_Due_Amount).ToList();

                Session["Total_Due_Amount"] = DueCllctedamt[0].Total_Due_Amount;
                Session["CollectedAmount"] = DueCllctedamt[0].CollectedAmount;
                tempvals = DueCllctedamt[0].Total_Due_Amount + "," + DueCllctedamt[0].CollectedAmount;
                //tempvals = "50,20"; test
                ViewBag.Counts = tempvals;
                ViewBag.Colnames = tempColName.ToString();





                //var sCounts = db.Branches.Select(p => p.BranchID).ToList();

                //var sColnames = db.Branches.Select(p => p.BranchName).ToList();

                //tempColName = string.Join("\",\"",sColnames);
                //tempColName = "\"" + tempColName + "\"";
                //tempvals = string.Join(",", sCounts);
                //ViewBag.Counts = tempvals;
                //ViewBag.Colnames = tempColName.ToString();

                var sInvstAmt = db.PartnerDetails.Select(p => p.InvestmentAmt).ToList();

                var sInvColnames = db.PartnerDetails.Select(p => p.PrtnrName).ToList();

                tempColName = string.Join("\",\"", sInvColnames);
                tempColName = "\"" + tempColName + "\"";
                tempvals = string.Join(",", sInvstAmt);
                ViewBag.InvCounts = tempvals;
                ViewBag.InvColnames = tempColName.ToString();

                if (!string.IsNullOrEmpty(Convert.ToString(DueCllctedamt[0].Total_Due_Amount)) && !string.IsNullOrEmpty(Convert.ToString(DueCllctedamt[0].CollectedAmount)))
                {
                    if (DueCllctedamt[0].Total_Due_Amount == DueCllctedamt[0].CollectedAmount)
                    {
                        Session["DueColl"] = "0.00";
                    }
                    else
                    {
                        Session["DueColl"] = (Convert.ToDouble(DueCllctedamt[0].Total_Due_Amount) - Convert.ToDouble(DueCllctedamt[0].CollectedAmount));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in Index() Get" + ex.InnerException);
            }
            return View();
        }

    }
}
