using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    [Table("Auction")]
    public class Auction
    {
        public Guid id { get; set; }
        [Required]
        [Display(Name="Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Image")]

        public byte[] image { get; set; }
        [Display(Name = "Duration")]
        public int duration { get; set; }
        [Display(Name = "Starting price")]
        public decimal startPrice { get; set; }
        [Display(Name = "Current price")]
        public decimal currPrice { get; set; }
        [Display(Name = "Price in tokens")]
        public int tokenPrice { get; set; }
        [Display(Name = "Token value")]
        public decimal tokenValue { get; set; }
        [Required]
        [StringLength(3)]
        [Display(Name = "Currency")]
        public string currency { get; set; }
        [Display(Name = "Created")]
        public DateTime timeCreated { get; set; }
        [Display(Name = "Opened")]
        public DateTime? timeOpened { get; set; }
        [Display(Name = "Closed")]
        public DateTime? timeClosed { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name="User that created this auction")]
        public int? userIdCreate { get; set; }
        [Display(Name = "User")]
        public virtual User user { get; set; }
        [Display(Name = "Bids")]
        public virtual ICollection<Bid> bids { get; set; }
    }
}