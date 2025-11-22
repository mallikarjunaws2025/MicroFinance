using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Branch")]
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchID { get; set; }

        [StringLength(255)]
        public string BranchCode { get; set; }

        [StringLength(255)]
        public string BranchName { get; set; }

        [StringLength(255)]
        public string OpenDate { get; set; }

        [StringLength(255)]
        public string BAddress { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string State { get; set; }

        public int? PinCode { get; set; }

        public int? ManagerID { get; set; }

        // Navigation Properties
        [ForeignKey("ManagerID")]
        public virtual Staff Manager { get; set; }
        
        public virtual ICollection<TmpSummaryReportData> TmpSummaryReports { get; set; }
        public virtual ICollection<UserLog> UserLogs { get; set; }
    }
}