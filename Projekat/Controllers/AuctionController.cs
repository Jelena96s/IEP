using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;
using Projekat.Hubs;
using PagedList;
using WebGrease.Css.Ast.Selectors;
using Projekat.Helpers;

namespace Projekat.Controllers
{
    public class AuctionController : Controller
    {
        private AuctionDb db = new AuctionDb();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogController));
        // GET: Auction
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser]
        public ActionResult CreateNewAuction()
        {
            return View();
        }

     
        public ActionResult DetailAuction(Auction auction)
        {
            return View(auction);
        }
       
        public ActionResult DetailAuctionFromAllAuctions(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Guid Sid = Guid.Parse(id);
            Auction auction = (Auction)db.auctions.Where(a => a.id == Sid).FirstOrDefault();
            return View("DetailAuction", auction);
        }


        [HttpPost]
        [AuthorizeUser]
        public ActionResult CreateNewAuction(CreateNewAuction model)
        {
            if (ModelState.IsValid)
            {
                User user = Session["User"] as User;
                Auction auction = new Auction();
                AdminSettings admin = db.adminSettings.FirstOrDefault();
                auction.id = Guid.NewGuid();
                auction.name = model.name;
                auction.duration = model.duration;
                auction.startPrice = model.startPrice;
                auction.currPrice = model.startPrice;
                auction.currency = admin.C;


                byte[] image = new byte[model.image.ContentLength];
                model.image.InputStream.Read(image, 0, image.Length);
                auction.image = image;
                auction.status = "READY";
                auction.timeCreated = DateTime.Now;
                auction.timeOpened = null;
                auction.timeClosed = null;
                auction.userIdCreate = user.id;
                auction.tokenValue = admin.T;
                auction.tokenPrice = (int)Math.Ceiling(auction.startPrice / auction.tokenValue);

                db.auctions.Add(auction);
                db.SaveChanges();
                return View("DetailAuction", auction);
            }

            return View(model);
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult EndOfAuction(Guid? id)
        {
            using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    Auction auction = db.auctions.Where(a => a.id == id).FirstOrDefault();
                    auction.status = "COMPLETED";
                    auction.timeClosed = System.DateTime.Now;
                    db.Entry(auction).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    if (auction.bids.Count != 0)
                    {
                        User sendTo = auction.bids.LastOrDefault().user;
                        string subject = "Get ordered product";
                        string body =
                            "You have won on the auction. Soon you will your ordered product will be sent to you. ";
                        string mailTo = sendTo.mail;
                        EmailClass.Email(mailTo, subject, body);
                        var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubProjekat>();
                        hubContext.Clients.All.updateWinner(auction.id, sendTo.name, sendTo.lastname);
                    }

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    log.Error($"Error-not sent mail to winner on {DateTime.Now}");
                }

            }
        return RedirectToAction("AllAuctions", "Home");
        }

        [HttpGet]
        [AuthorizeUser]
        public ActionResult Won(string CurrentFilter, string searchStringOrPartOfString, int? priceFrom, int? priceTo, int? itemsPerPage, string wayOfSorting, int? page)
        {
            if (searchStringOrPartOfString != null)
            {
                page = 1;
            }
            else
            {
                searchStringOrPartOfString = CurrentFilter;
            }

            ViewBag.CurrentFilter = searchStringOrPartOfString;

            ViewBag.itemsPerPage = itemsPerPage != null ? itemsPerPage : db.adminSettings.FirstOrDefault().N;


            ViewBag.Message = "All auctions:";
            User user = Session["User"] as User;
            User user1 = db.users.Where(u => u.id == user.id).FirstOrDefault();

            var auctions = from a in db.auctions
                from b in db.bids
                where b.userId == user1.id && b.auctionId == a.id && a.status == "COMPLETED"
                select a;


              switch (wayOfSorting)
            {
                case "ascending":
                    auctions = auctions.OrderBy(a => a.name);
                    break;
                case "descending":
                    auctions = auctions.OrderByDescending(a => a.name);
                    break;
                case "price_asc":
                    auctions = auctions.OrderBy(a => a.currPrice);
                    break;
                case "price_desc":
                    auctions = auctions.OrderByDescending(a => a.currPrice);
                    break;
                case "READY":
                    auctions = auctions.Where(a => a.status == "READY");
                    break;
                case "OPENED":
                    auctions = auctions.Where(a => a.status == "OPENED");
                    break;
                case "COMPLETED":
                    auctions = auctions.Where(a => a.status == "COMPLETED");
                    break;
                default:
                    auctions = auctions.OrderBy(a => a.timeOpened).Take(10);
                    break;
            }

            ViewBag.wayOfSorting = wayOfSorting;
            ViewBag.priceFrom = priceFrom;
            ViewBag.priceTo = priceTo;
            ViewBag.searchStringOrPartOfString = searchStringOrPartOfString;
            if (priceFrom != null)
                auctions = auctions.Where(a => a.currPrice > priceFrom);
            if (priceTo != null)
                auctions = auctions.Where(a => a.currPrice < priceTo);
            if (searchStringOrPartOfString != null && searchStringOrPartOfString != "")
            {
                var listOfWords = searchStringOrPartOfString.Split(' ');
                foreach (var oneWord in listOfWords)
                    auctions = auctions.Where(a => a.name.ToLower().Contains(oneWord.ToLower()));
            }
            int pageNumber = (page ?? 1);
            return View(PagedListExtensions.ToPagedList(auctions.ToList().Distinct(), pageNumber, ViewBag.itemsPerPage));
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