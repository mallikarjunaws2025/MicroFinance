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
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;
            
            // Set comprehensive cache control headers to prevent back button access after logout
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            response.Cache.SetNoStore();
            response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetNoServerCaching();
            response.Headers.Remove("Cache-Control");
            response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate, max-age=0, private");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");
            
            // Check if session exists and is valid
            var userId = session["UserID"];
            var validUser = session["ValideUsr"];
            
            // More strict session validation
            if (userId == null || 
                string.IsNullOrEmpty(userId.ToString()) ||
                validUser == null ||
                string.IsNullOrEmpty(validUser.ToString()) ||
                !Helper.IsValidUser(validUser.ToString()))
            {
                // Clear any remaining session data
                session.Clear();
                session.Abandon();
                
                // Redirect to login with cache busting
                var loginUrl = "~/Staff/StaffLogin?expired=1&t=" + DateTime.Now.Ticks;
                filterContext.Result = new RedirectResult(loginUrl);
                return;
            }

            // Additional validation: check if helper state matches session
            if (!Helper.bIsValidUser)
            {
                session.Clear();
                session.Abandon();
                filterContext.Result = new RedirectResult("~/Staff/StaffLogin?invalid=1");
                return;
            }

            // Check admin requirements
            if (RequireAdmin)
            {
                string userRole = Convert.ToString(session["UsrType"]);
                if (string.IsNullOrEmpty(userRole) || !userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    filterContext.Result = new RedirectResult("~/Charts/Index_New?error=unauthorized");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}