using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("Staff")]
    public class Staff
    {
        public Staff()
        {
            StaffLogins = new HashSet<StaffLogin>();
            ManagedBranches = new HashSet<Branch>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffID { get; set; }

        [StringLength(255)]
        public string StaffName { get; set; }

        [StringLength(255)]
        public string DOB { get; set; }

        [StringLength(255)]
        public string DOJ { get; set; }

        [StringLength(6)]
        public string Status { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        // Navigation Properties
        public virtual ICollection<StaffLogin> StaffLogins { get; set; }
        public virtual ICollection<Branch> ManagedBranches { get; set; }
    }
}