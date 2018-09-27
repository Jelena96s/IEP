using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Projekat.Helpers;

namespace Projekat.Controllers
{
    public class HomeController : Controller
    {
        private AuctionDb db = new AuctionDb();
      
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
    /*    [HttpPost]
        public ActionResult AllAuctions()
        {
            var auctions = from a in db.auctions select a;
            if (auctions == null)
                return RedirectToAction("Index", "Home");
            ViewBag.Message = "All auctions:";
            return View(auctions);
        }
        */
        [HttpGet]
        public ActionResult AllAuctions(string CurrentFilter, string searchStringOrPartOfString, int? priceFrom,
            int? priceTo, int? itemsPerPage, string wayOfSorting, int? page)
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
            var auctions = from a in db.auctions select a;
            if (auctions != null)
            {
                
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
            }

            if (auctions == null)
                return RedirectToAction("Index", "Home");
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