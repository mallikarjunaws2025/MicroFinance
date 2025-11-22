using System;
using ManageFinancery.DatabaseMigration;
using ManageFinancery.TestApp;

namespace ManageFinancery
{
    /// <summary>
    /// Database Migration Program
    /// Run this to create the new Model First database
    /// </summary>
    class DatabaseMigrationProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MicroFinance Model First Database Migration ===");
            Console.WriteLine();
            
            try
            {
                // Check if this is a test run
                bool isTest = args.Length > 0 && args[0].ToLower() == "-test";
                
                if (isTest)
                {
                    Console.WriteLine("Running in TEST mode...");
                    TestModelFirst.RunTests();
                }
                else
                {
                    Console.WriteLine("Starting database creation...");
                    DatabaseInitializer.InitializeDatabase();
                    Console.WriteLine("✓ Database migration completed successfully!");
                    Console.WriteLine();
                    Console.WriteLine("The new Model First database 'MicroFinance_ModelFirst' has been created.");
                    Console.WriteLine("You can now switch your application to use the new Model First approach.");
                    
                    // Ask if user wants to run tests
                    Console.WriteLine();
                    Console.Write("Would you like to run tests now? (y/N): ");
                    var response = Console.ReadLine();
                    if (response?.ToLower() == "y")
                    {
                        Console.WriteLine();
                        TestModelFirst.RunTests();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Operation failed: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}