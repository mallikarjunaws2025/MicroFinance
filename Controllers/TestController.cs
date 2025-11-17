using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using TestApp.DB;

namespace TestApp.Controllers
{
    public class TestController : Controller
    {
        public ActionResult DatabaseTest()
        {
            // Test 1: Direct SQL Connection
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MicroFinance;Integrated Security=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ViewBag.SqlConnectionTest = "✓ Direct SQL Connection successful";
                    
                    // Test basic query
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM StaffLogin", connection))
                    {
                        var count = command.ExecuteScalar();
                        ViewBag.DirectSqlCount = $"Direct SQL - StaffLogin count: {count}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.SqlConnectionTest = $"✗ Direct SQL Connection failed: {ex.Message}";
            }
            
            // Test 2: Entity Framework Connection
            try
            {
                using (var db = new MicroFinanceEntities())
                {
                    ViewBag.EfConnectionTest = "✓ Entity Framework connection created";
                    
                    // Test basic queries
                    var staffCount = db.Staffs.Count();
                    var loginCount = db.StaffLogins.Count();
                    
                    ViewBag.StaffCount = $"EF - Staff count: {staffCount}";
                    ViewBag.LoginCount = $"EF - Login count: {loginCount}";
                    
                    // Test if we can fetch staff login data
                    var adminUser = db.StaffLogins.FirstOrDefault(x => x.UserName == "admin");
                    if (adminUser != null)
                    {
                        ViewBag.AdminFound = $"✓ Admin user found: {adminUser.UserName} (Role: {adminUser.RolePermission})";
                    }
                    else
                    {
                        ViewBag.AdminFound = "✗ Admin user not found";
                    }
                    
                    ViewBag.EfSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.EfSuccess = false;
                ViewBag.EfError = $"Entity Framework Error: {ex.Message}";
                ViewBag.EfInnerError = ex.InnerException?.Message;
            }
            
            // Get connection string info
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["MicroFinanceEntities"];
            ViewBag.ConnectionStringName = connectionStringSettings?.Name ?? "Not found";
            ViewBag.ConnectionString = connectionStringSettings?.ConnectionString ?? "Not found";
            ViewBag.ProviderName = connectionStringSettings?.ProviderName ?? "Not found";
            
            return View();
        }
    }
}