using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("FinGroup")]
    public class FinGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupID { get; set; }

        [StringLength(255)]
        public string GroupCode { get; set; }

        [StringLength(255)]
        public string BranchCode { get; set; }

        [StringLength(255)]
        public string Village { get; set; }

        public int? Net_Members { get; set; }

        [StringLength(255)]
        public string CrDt { get; set; }

        [StringLength(255)]
        public string DOC { get; set; }

        public int? Distance { get; set; }

        [StringLength(255)]
        public string GStatus { get; set; }

        public int? StaffID { get; set; }

        [StringLength(255)]
        public string FormedBy { get; set; }

        [StringLength(255)]
        public string MeetingPlaceAddress { get; set; }

        [StringLength(255)]
        public string GType { get; set; }

        [StringLength(255)]
        public string GrpName { get; set; }

        [StringLength(10)]
        public string RecoveryDay { get; set; }
    }
}