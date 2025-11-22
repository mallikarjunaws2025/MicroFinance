using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    /// <summary>
    /// UserLog entity matching exact database schema
    /// </summary>
    [Table("UserLog")]
    public class UserLog
    {
        [Key]
        public int LogId { get; set; }
        
        public string LoginName { get; set; }
        
        public DateTime? LogInDateTime { get; set; }
        
        public DateTime? LogOutDateTime { get; set; }
        
        public string StaffName { get; set; }
    }
}