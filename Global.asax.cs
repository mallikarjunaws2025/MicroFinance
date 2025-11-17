using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TestApp.DB;
using TestApp.Comman;

namespace TestApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            // Initialize Entity Framework for Database First
            Database.SetInitializer<MicroFinanceEntities>(null);
            
            // Test database connection and create default admin if needed
            InitializeDatabase();
        }
        
        private void InitializeDatabase()
        {
            try
            {
                using (var db = new MicroFinanceEntities())
                {
                    // Test connection and ensure we can access the database
                    bool canConnect = db.Database.Exists();
                    System.Diagnostics.Debug.WriteLine($"Database connection test: {canConnect}");
                    
                    // Check if admin user exists, create if not
                    if (canConnect && !db.StaffLogins.Any(x => x.UserName == "admin"))
                    {
                        CreateDefaultAdmin(db);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }
        
        private void CreateDefaultAdmin(MicroFinanceEntities db)
        {
            try
            {
                // Create default admin staff
                var adminStaff = new Staff
                {
                    StaffName = "Administrator",
                    DOB = DateTime.Now.AddYears(-30).ToString("dd/MM/yyyy"),
                    DOJ = DateTime.Now.ToString("dd/MM/yyyy"),
                    Status = "Active"
                };
                db.Staffs.Add(adminStaff);
                db.SaveChanges();
                
                // Create default admin login
                var adminLogin = new StaffLogin
                {
                    StaffId = adminStaff.StaffID,
                    UserName = "admin",
                    Password = Helper.HashPassword("admin123"), // Default password
                    RolePermission = "Admin",
                    IsLocked = 0
                };
                db.StaffLogins.Add(adminLogin);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating default admin: {ex.Message}");
            }
        }
    }
}