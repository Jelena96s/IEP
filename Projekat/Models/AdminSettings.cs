using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class AdminSettings
    {
        public int id { get; set; }
       [Display(Name="Number of auctions per page")]
        public int N { get; set; }
        [Display(Name = "Default duration of auction")]
        public int D { get; set; }
        [Display(Name = "Number of tokens for silver pack")]
        public int S { get; set; }
        [Display(Name = "Number of tokens for golden pack")]
        public int G { get; set; }
        [Display(Name = "Number of tokens for platinum pack")]
        public int P { get; set; }
        [Display(Name = "Currency")]
        [StringLength(3)]
        public string C { get; set; }
        [Display(Name = "Token value")]
        public decimal T { get; set; }
    }
}