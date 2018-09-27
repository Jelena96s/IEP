using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class CreateNewAuction
    {
        [Required]
        [Display(Name = "Name of the product")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Insert image of the product")]
        public HttpPostedFileBase image { get; set; }
        public int duration { get; set; }
        public decimal startPrice { get; set; }
      
    }
}