using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApp.VModels.Management
{
    public class AddNewPartner
    {
            public string PrtnrName { get; set; }
            public string InvestmentAmt { get; set; }

            public string Address { get; set; }
            public string CantactNum { get; set; }
            public string City { get; set; }
            public string Statte { get; set; }
            public string ZIP { get; set; }

            public string AdharaCardNum { get; set; }
            
            public string DOJ { get; set; }
            [Required(ErrorMessage = "Please Enter DOJ")]
            public Int32 DOJD { get; set; }
            [Required(ErrorMessage = "Please Enter DOJ")]
            public Int32 DOJM { get; set; }
            [Required(ErrorMessage = "Please Enter DOJ")]
            public Int32 DOJY { get; set; }

           
            public string IsSucess { get; set; }
        
    }
}