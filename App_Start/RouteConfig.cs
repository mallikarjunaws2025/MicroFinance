using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Staff", action = "StaffLogin", id = UrlParameter.Optional }
//                 defaults: new { controller = "Loans", action = "LoanLists", id = UrlParameter.Optional } 
                    //defaults: new { controller = "Member", action = "MemberDetails", id = UrlParameter.Optional } 
            );
        }
    }
}