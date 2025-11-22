using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Loans")]
    public class Loan
    {
        public Loan()
        {
            LoanCols = new HashSet<Loan_Cols>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanID { get; set; }

        public int? MbrId { get; set; }

        [StringLength(255)]
        public string Loan_Amount { get; set; }

        [StringLength(255)]
        public string Balance_Amount { get; set; }

        [StringLength(255)]
        public string RateOfInterest { get; set; }

        [StringLength(255)]
        public string Prin_EMI { get; set; }

        [StringLength(255)]
        public string Int_EMI { get; set; }

        public int? NoOfDay { get; set; }

        [StringLength(255)]
        public string Date_Of_Disbursement { get; set; }

        [StringLength(255)]
        public string Expiry_Date { get; set; }

        [StringLength(255)]
        public string ALRSavings { get; set; }

        [StringLength(255)]
        public string GRFAmt { get; set; }

        [StringLength(255)]
        public string Processing_Fee { get; set; }

        public int? Stationary { get; set; }

        public int? Insurance { get; set; }

        [StringLength(255)]
        public string Other_Income { get; set; }

        [StringLength(255)]
        public string NetSavings { get; set; }

        [StringLength(20)]
        public string PostedUserID { get; set; }

        // Navigation Properties
        [ForeignKey("MbrId")]
        public virtual Member Member { get; set; }
        public virtual ICollection<Loan_Cols> LoanCols { get; set; }
    }
}