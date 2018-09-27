using Microsoft.AspNet.SignalR;
using Projekat.Helpers;
using Projekat.Hubs;
using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class AdminController : Controller
    {
        private AuctionDb db = new AuctionDb();
        // GET: Admin
        [AuthorizeAdmin]
        public ActionResult OpenAuction()
        {
            return RedirectToAction("AllAuctions", "Home");
        }
        [AuthorizeAdmin]
        public ActionResult ChangeStatus(Guid id)
        {
            Auction auction = db.auctions.Where(a => a.id == id).FirstOrDefault();
            auction.status = "OPENED";
            auction.timeOpened = System.DateTime.Now;
            db.Entry(auction).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AllAuctions", "Home");
        }
        [AuthorizeAdmin]
        public ActionResult ChangeParameters()
        {
            AdminSettings admin = db.adminSettings.FirstOrDefault();
            return View(admin);
        }
        [AuthorizeAdmin]
        public ActionResult Parameters(AdminSettings model)
        {
            if (ModelState.IsValid)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<HubProjekat>();
                AdminSettings admin = db.adminSettings.FirstOrDefault();
                if (model.N != 0)
                {
                    admin.N = model.N;
                }

                if (model.D != 0)
                {
                    admin.D = model.D;
                }

                if (model.S != 0)
                {
                    admin.S = model.S;
                }

                if (model.G != 0)
                {
                    admin.G = model.G;
                }

                if (model.P != 0)
                {
                    admin.P = model.P;
                }

                if (model.C != null)
                {
                    admin.C = model.C;

      //              hubContext.Clients.All.updateCurrency(admin.C);
       //             hubContext.Clients.All.updateCurrency1(admin.C);
                }
                if (model.T != 0)
                {
                    admin.T = model.T;
       //             hubContext.Clients.All.updateTokenValue(admin.T);
                }
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return View(admin);
            }
            return View(model);
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