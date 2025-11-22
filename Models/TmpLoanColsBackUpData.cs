using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    /// <summary>
    /// TmpLoanColsBackUpData entity matching exact database schema
    /// </summary>
    [Table("TmpLoanColsBackUpData")]
    public class TmpLoanColsBackUpData
    {
        [Key]
        public int LoanColId { get; set; }
    }
}