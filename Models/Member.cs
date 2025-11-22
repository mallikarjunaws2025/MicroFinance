using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Member")]
    public class Member
    {
        public Member()
        {
            Loans = new HashSet<Loan>();
            LoanCols = new HashSet<Loan_Cols>();
            ALRAudjustments = new HashSet<ALRAudjustment>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MbrId { get; set; }

        [StringLength(255)]
        public string MbrName { get; set; }

        [StringLength(255)]
        public string GroupCode { get; set; }

        [StringLength(255)]
        public string Husbandname { get; set; }

        public int? Age { get; set; }

        [StringLength(10)]
        public string MbrStatus { get; set; }

        [StringLength(255)]
        public string WithdrawDt { get; set; }

        public int? StaffId { get; set; }

        [StringLength(255)]
        public string MbrAddress { get; set; }

        [StringLength(255)]
        public string DOJ { get; set; }

        [StringLength(255)]
        public string CantactNo { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(255)]
        public string GrpName { get; set; }

        [StringLength(255)]
        public string StaffName { get; set; }

        [StringLength(50)]
        public string Nominee { get; set; }

        [StringLength(12)]
        public string PhoneNo2 { get; set; }

        [StringLength(20)]
        public string AdharaCardNum { get; set; }

        [StringLength(15)]
        public string RCardNo { get; set; }

        [StringLength(20)]
        public string RCardNo1 { get; set; }

        // Navigation Properties
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<Loan_Cols> LoanCols { get; set; }
        public virtual ICollection<ALRAudjustment> ALRAudjustments { get; set; }
    }
}