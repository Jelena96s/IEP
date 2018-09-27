using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projekat.Hubs;
using Microsoft.AspNet.SignalR;
using Projekat.Helpers;

namespace Projekat.Controllers
{
    public class BidController : Controller
    {
        private AuctionDb db = new AuctionDb();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogController));

        // GET: Bid
        [AuthorizeUser]
        public ActionResult Bid(Guid? auctionId, int? bidOffer)
        {
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                try { 
                    if (auctionId != null && bidOffer != null)
                    {
                        Auction auction = db.auctions.Where(a => a.id == auctionId).FirstOrDefault();
                        Bid bidToCompare = auction.bids.OrderByDescending(b => b.timeSent).FirstOrDefault();
                        int oldOffer = 0;
                        if (bidToCompare != null)
                        {
                            oldOffer = bidToCompare.numOfTokens; 
                        }
                        else
                        {
                            oldOffer = auction.tokenPrice;
                    }
                        User user = Session["User"] as User;
                        User user1 = db.users.Where(u => u.id == user.id).FirstOrDefault();
                        if (bidOffer > oldOffer && bidOffer <= user.numOfTokens && user.id != auction.userIdCreate)
                        {
                            Bid bid = new Bid
                            {
                                userId = user.id,
                                timeSent = System.DateTime.Now,
                                auctionId = auction.id,
                                numOfTokens = (int) bidOffer
                            };
                            db.bids.Add(bid);

                            user1.numOfTokens = user1.numOfTokens - (int) bidOffer;
                            db.Entry(user1).State = EntityState.Modified;
                            AdminSettings admin = db.adminSettings.FirstOrDefault();
                            auction.currPrice = (decimal) bidOffer * admin.T;
                            auction.tokenPrice = (int) bidOffer;
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubProjekat>();
                            hubContext.Clients.All.updatePage(auction.id, auction.currPrice, auction.currency, user1.mail, auction.tokenPrice);
                            hubContext.Clients.All.bidUp(user1.name, user1.lastname, bid.timeSent, bid.numOfTokens, auction.currPrice, auction.id);
                            hubContext.Clients.All.userUp(user1.numOfTokens, user1.id);
                            db.Entry(auction).State = EntityState.Modified;
                            db.SaveChanges();
                            transaction.Commit();
                           
                    }
                    }
            }
            catch (Exception e)
            {
                transaction.Rollback();
                ViewBag.Message = "There is an error in transaction";
                log.Error($"Error-not finished bid on {DateTime.Now}");
            }
            }
          
        //    var auctions = from a in db.auctions select a;
            return RedirectToAction("AllAuctions", "Home"); //, auctions.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}