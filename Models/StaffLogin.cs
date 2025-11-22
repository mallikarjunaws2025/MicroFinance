using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    [Table("StaffLogin")]
    public class StaffLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsrId { get; set; }

        public int? StaffId { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(255)]
        public string RolePermission { get; set; }

        public int? IsLocked { get; set; }

        // Navigation Properties
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }
    }
}