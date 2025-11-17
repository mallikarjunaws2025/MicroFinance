using System;
using System.Web;
using System.Web.Mvc;
using TestApp.Comman;

namespace TestApp.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public bool RequireAdmin { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            
            // Check if user is logged in
            if (!Helper.IsValidUser(Convert.ToString(session["ValideUsr"])))
            {
                filterContext.Result = new RedirectResult("~/Staff/StaffLogin");
                return;
            }

            // Check admin requirements
            if (RequireAdmin)
            {
                string userRole = Convert.ToString(session["UsrType"]);
                if (string.IsNullOrEmpty(userRole) || !userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    filterContext.Result = new RedirectResult("~/Charts/Index?error=unauthorized");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}