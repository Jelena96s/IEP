using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    [Table("User")]
    public class User
    {
        [Display(Name = "Id")]
        public int id { get; set; }
        [Required]
        [StringLength((50))]
        [Display(Name="Name")]
        public string name { get; set; }
        [Required]
        [StringLength((50))]
        [Display(Name = "Lastname")]
        public string lastname { get; set; }
        [Required]
        [StringLength((50))]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Mail")]
        public string mail { get; set; }
        [Required]
        [StringLength((50))]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Display(Name = "Number of tokens")]
        public int numOfTokens { get; set; }
        [Required]
        [Display(Name = "Am I admin")]
        public int isAdmin { get; set; }

        public virtual ICollection<TokenOrder> tokenOrders{ get; set; }

        public virtual ICollection<Bid> bids { get; set; }

        public virtual ICollection<Auction> auctions { get; set; }
    }
   
}