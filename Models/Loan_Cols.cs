using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Loan_Cols")]
    public class Loan_Cols
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanColId { get; set; }

        public int? LoanId { get; set; }

        public int? MbrId { get; set; }

        [StringLength(255)]
        public string Collected_Amount { get; set; }

        [StringLength(255)]
        public string Actual_Balance { get; set; }

        [StringLength(255)]
        public string Prin_Balance { get; set; }

        [StringLength(255)]
        public string Int_Collect { get; set; }

        [StringLength(255)]
        public string Prin_Collected { get; set; }

        public int? Balance_Installment { get; set; }

        [StringLength(255)]
        public string Balance_Interest { get; set; }

        [StringLength(255)]
        public string Prin_Due { get; set; }

        [StringLength(255)]
        public string Int_Due { get; set; }

        [StringLength(255)]
        public string Transact_Date { get; set; }

        [StringLength(255)]
        public string Next_Due_Date { get; set; }

        [StringLength(255)]
        public string Upto_Last_Savings { get; set; }

        [StringLength(255)]
        public string ALRSavings { get; set; }

        [StringLength(255)]
        public string As_On { get; set; }

        [StringLength(255)]
        public string Adjustment { get; set; }

        [StringLength(255)]
        public string PaidEMI { get; set; }

        [StringLength(255)]
        public string PrePrinAmt { get; set; }

        [StringLength(255)]
        public string PreInterestAmt { get; set; }

        [StringLength(255)]
        public string Collect_Or_RefoundAmt { get; set; }

        [StringLength(20)]
        public string PostedUserID { get; set; }

        // Navigation Properties
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; }

        [ForeignKey("MbrId")]
        public virtual Member Member { get; set; }
    }
}