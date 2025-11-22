using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using TestApp.Models;

namespace TestApp.Data
{
    public class MicroFinanceContext : DbContext
    {
        public MicroFinanceContext() : base("name=MicroFinanceModelFirst")
        {
            // Enable automatic migrations for development
            Database.SetInitializer(new CreateDatabaseIfNotExists<MicroFinanceContext>());
        }

        // DbSets for all entities
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Loan_Cols> LoanCols { get; set; }
        public DbSet<ALRAudjustment> ALRAudjustments { get; set; }
        public DbSet<FinGroup> FinGroups { get; set; }
        public DbSet<Management> Managements { get; set; }
        public DbSet<PartnerDetail> PartnerDetails { get; set; }
        public DbSet<StaffLogin> StaffLogins { get; set; }
        
        // Temporary/Backup Tables
        public DbSet<TmpLoanColsBackUpData> TmpLoanColsBackUpData { get; set; }
        public DbSet<TmpLoansBackUpData> TmpLoansBackUpData { get; set; }
        public DbSet<TmpReceipts> TmpReceipts { get; set; }
        public DbSet<TmpSummaryReportData> TmpSummaryReportData { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralizing table name convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configure Staff relationships
            modelBuilder.Entity<Staff>()
                .HasMany(s => s.StaffLogins)
                .WithOptional(sl => sl.Staff)
                .HasForeignKey(sl => sl.StaffId);

            modelBuilder.Entity<Staff>()
                .HasMany(s => s.ManagedBranches)
                .WithOptional(b => b.Manager)
                .HasForeignKey(b => b.ManagerID);

            // Configure Member relationships
            modelBuilder.Entity<Member>()
                .HasMany(m => m.Loans)
                .WithOptional(l => l.Member)
                .HasForeignKey(l => l.MbrId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.LoanCols)
                .WithOptional(lc => lc.Member)
                .HasForeignKey(lc => lc.MbrId);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.ALRAudjustments)
                .WithOptional(a => a.Member)
                .HasForeignKey(a => a.MbrID);

            // Configure Loan relationships
            modelBuilder.Entity<Loan>()
                .HasMany(l => l.LoanCols)
                .WithOptional(lc => lc.Loan)
                .HasForeignKey(lc => lc.LoanId);

            // Configure Management relationships
            modelBuilder.Entity<PartnerDetail>()
                .HasMany(pd => pd.Managements)
                .WithOptional(m => m.PartnerDetail)
                .HasForeignKey(m => m.PrtnrID);

            // Configure property specific settings
            modelBuilder.Entity<Staff>()
                .Property(s => s.StaffName)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(m => m.RCardNo1)
                .HasMaxLength(20);

            // TmpLoanColsBackUpData Configuration - Simple table, no relationships
            // TmpLoansBackUpData Configuration - Simple table, no relationships  
            // TmpReceipts Configuration - Simple table, no relationships
            // TmpSummaryReportData Configuration - Simple table, no relationships
            // UserLog Configuration - Simple table, no relationships

            base.OnModelCreating(modelBuilder);
        }
    }
}