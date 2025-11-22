using System;
using System.Data.Entity;
using ManageFinancery.Data;

namespace ManageFinancery.DatabaseMigration
{
    /// <summary>
    /// Database initializer for creating the new Model First database
    /// </summary>
    public class DatabaseInitializer
    {
        public static void InitializeDatabase()
        {
            try
            {
                // Set the database initializer to create database if not exists
                Database.SetInitializer(new CreateDatabaseIfNotExists<MicroFinanceContext>());
                
                using (var context = new MicroFinanceContext())
                {
                    // This will trigger database creation
                    context.Database.Initialize(force: false);
                    Console.WriteLine("Database created successfully!");
                    
                    // Optional: Add some seed data here if needed
                    SeedData(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database: {ex.Message}");
                throw;
            }
        }
        
        private static void SeedData(MicroFinanceContext context)
        {
            // Add any initial data here if needed
            // For now, just ensure the database structure is created
            Console.WriteLine("Database structure created successfully!");
        }
    }
    
    /// <summary>
    /// Custom database initializer that drops and creates database
    /// Use this if you want to recreate the database from scratch
    /// </summary>
    public class DropCreateDatabaseInitializer : DropCreateDatabaseAlways<MicroFinanceContext>
    {
        protected override void Seed(MicroFinanceContext context)
        {
            // Add seed data here if needed
            base.Seed(context);
        }
    }
}