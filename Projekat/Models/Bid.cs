using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    [Table("Bid")]
    public class Bid
    {
        public int id { get; set; }
        public Guid auctionId { get; set; }
        public int userId { get; set; }
        public DateTime timeSent { get; set; }
        public int numOfTokens { get; set; }

        public virtual User user { get; set; }
        public virtual Auction auction { get; set; }
    }
}