using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    /// <summary>
    /// TmpReceipts entity matching exact database schema
    /// </summary>
    [Table("TmpReceipts")]
    public class TmpReceipts
    {
        [Key]
        public int SlNo { get; set; }
        
        public string Receipts { get; set; }
        
        public string Rs { get; set; }
    }
}