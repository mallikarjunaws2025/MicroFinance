using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Management")]
    public class Management
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvestID { get; set; }

        public int? PrtnrID { get; set; }

        [StringLength(255)]
        public string InvesterName { get; set; }

        [StringLength(255)]
        public string InvestmentAmt { get; set; }

        [StringLength(255)]
        public string WithdrawAmt { get; set; }

        [StringLength(255)]
        public string BalanceAmt { get; set; }

        [StringLength(255)]
        public string WithdrawDate { get; set; }

        [StringLength(255)]
        public string ProfitAmt { get; set; }

        [StringLength(255)]
        public string LossAmt { get; set; }

        [StringLength(255)]
        public string DueAmt { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        [StringLength(255)]
        public string CrDate { get; set; }

        // Navigation Properties
        [ForeignKey("PrtnrID")]
        public virtual PartnerDetail PartnerDetail { get; set; }
    }
}