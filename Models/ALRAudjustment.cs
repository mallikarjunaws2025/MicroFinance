using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("ALRAudjustments")]
    public class ALRAudjustment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ALRID { get; set; }

        public int? MbrID { get; set; }

        [StringLength(255)]
        public string ALRSavings { get; set; }

        [StringLength(255)]
        public string AdvanceALR { get; set; }

        [StringLength(255)]
        public string CrDt { get; set; }

        // Navigation Properties
        [ForeignKey("MbrID")]
        public virtual Member Member { get; set; }
    }
}