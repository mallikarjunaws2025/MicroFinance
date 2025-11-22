using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("PartnerDetail")]
    public class PartnerDetail
    {
        public PartnerDetail()
        {
            Managements = new HashSet<Management>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrtnrID { get; set; }

        [StringLength(255)]
        public string PrtnrName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(255)]
        public string CatntactNum { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string Statte { get; set; }

        [StringLength(255)]
        public string ZIP { get; set; }

        [StringLength(20)]
        public string AdharaCardNum { get; set; }

        [StringLength(255)]
        public string CrDate { get; set; }

        [StringLength(255)]
        public string InvestmentAmt { get; set; }

        // Navigation Properties
        public virtual ICollection<Management> Managements { get; set; }
    }
}