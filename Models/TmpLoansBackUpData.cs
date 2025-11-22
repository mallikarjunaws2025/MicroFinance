using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    /// <summary>
    /// Temporary backup data for loans
    /// </summary>
    [Table("TmpLoansBackUpData")]
    public class TmpLoansBackUpData
    {
        [Key]
        public int BackupId { get; set; }
        
        public int? LoanId { get; set; }
        
        public int? MemberId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestRate { get; set; }
        
        public int? Duration { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; }
        
        [StringLength(500)]
        public string Remarks { get; set; }
        
        public int? StaffId { get; set; }
        
        public DateTime? BackupDate { get; set; }
        
        [StringLength(50)]
        public string BackupReason { get; set; }
        
        // Navigation properties
        public virtual Loan Loan { get; set; }
        public virtual Member Member { get; set; }
        public virtual Staff Staff { get; set; }
    }
}