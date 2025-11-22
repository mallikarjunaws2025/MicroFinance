using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    /// <summary>
    /// TmpSummeryReportData entity matching exact database schema
    /// </summary>
    [Table("TmpSummeryReportData")]
    public class TmpSummaryReportData
    {
        [Key]
        public int SlNo { get; set; }
        
        public string Particulars { get; set; }
        
        public string BOR { get; set; }
        
        public string Added { get; set; }
        
        public string Dropped { get; set; }
        
        public string EOR { get; set; }
    }
}