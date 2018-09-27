using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projekat.Models;
using System.Data;
using System.Data.Entity;
using PagedList;
using Projekat.Helpers;
using Microsoft.AspNet.SignalR;
using Projekat.Hubs;

namespace Projekat.Controllers
{
    public class TokenOrderController : Controller
    {
        private AuctionDb db = new AuctionDb();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogController));

        // GET: TokenOrder
        [AuthorizeUser]
        public ActionResult TokenOrderList()
        {
            AdminSettings admin = db.adminSettings.FirstOrDefault();
            return View(admin);
        }
        [AuthorizeUser]
        public ActionResult TokenOrders(int? page)
        {
            User user = Session["User"] as User;
            IList<TokenOrder> orders = db.tokenOrders.Where(t => t.userId == user.id).OrderByDescending(t => t.dateSubmitted).ToList();
            AdminSettings admin = db.adminSettings.FirstOrDefault();

            int pageNumber = (page ?? 1);
            return View(PagedListExtensions.ToPagedList(orders, pageNumber, admin.N));
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult Order(string tokens)
        {
            AdminSettings admin = db.adminSettings.FirstOrDefault();
            User user = Session["User"] as User;
            int tokensNum = 0;
            if (tokens == "silver")
                tokensNum = admin.S;
            if (tokens == "gold")
                tokensNum = admin.G;
            if (tokens == "platinum")
                tokensNum = admin.P;
           
            TokenOrder order = new TokenOrder{id = Guid.NewGuid(), dateSubmitted = System.DateTime.Now, type = tokens, userId = user.id,numOfTokens = tokensNum,price = tokensNum * admin.T,status = "SUBMITTED"};
            db.tokenOrders.Add(order);
            db.SaveChanges();

            return Redirect("http://stage.centili.com/payment/widget?apikey=8603e733aca172f0f2472ecadffb5739&country=rs&reference=" + order.id + "&returnurl=http://sj150153.azurewebsites.net/TokenOrder/Completed");
        }
       
        public ActionResult Completed(Guid? reference, string status)
        {
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    TokenOrder order = db.tokenOrders.Where(t => t.id == reference).FirstOrDefault();
                    if (order == null)
                        RedirectToAction("TokenOrders", "TokenOrder");
                    if (order.status == "SUBMITTED")
                    {
                        if (status == "success")
                        {
                            order.status = "COMPLETED";
                            User user1 = Session["User"] as User;
                            User user = db.users.Where(u => u.id == user1.id).FirstOrDefault();
                            user.numOfTokens += order.numOfTokens;
                            var number = user.numOfTokens;
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubProjekat>();
                            hubContext.Clients.All.userUp1(user.numOfTokens, user.id);
                            db.Entry(user).State = EntityState.Modified;
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();
                            EmailClass.Email(user.mail, "Token order", "Token order completed");
                            transaction.Commit();
                          
                        }
                        else
                        {
                            order.status = "CANCELED";
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();
                            ViewBag.Mesage = "The order is canceled";
                            transaction.Commit();
                        }
                      
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ViewBag.Message = "There is an error in transaction";
                    log.Error($"Error-not completed token order on {DateTime.Now}");
                }
            }
            return RedirectToAction("TokenOrders", "TokenOrder");
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