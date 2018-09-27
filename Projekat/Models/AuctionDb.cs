using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class AuctionDb:DbContext
    {
        public virtual DbSet<Auction> auctions { get; set; }
        public virtual DbSet<Bid> bids { get; set; }

        public virtual DbSet<User> users { get; set; }
        public virtual DbSet<TokenOrder> tokenOrders { get; set; }
        public virtual DbSet<AdminSettings> adminSettings { get; set; }

        public AuctionDb()
            :base("name=AuctionDb")
        { }
    }
}