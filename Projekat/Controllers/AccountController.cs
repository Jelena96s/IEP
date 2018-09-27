using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Projekat.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using Projekat.Helpers;

namespace Projekat.Controllers
{
  //  [Authorize]
    public class AccountController : Controller
    {
        private AuctionDb db = new AuctionDb();
        
        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AuthorizeGuest]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AuthorizeGuest]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var passwordAddit = "";

                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(model.Password);
                    byte[] hashBytes = md5Hash.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    passwordAddit = sb.ToString();

                }

                User user = db.users.Where(u => u.mail == model.Email).FirstOrDefault();

                if (user == null)
                {
                    ViewBag.Message = "An account with the given email doesn't exist.";
                    return View(model);
                }

                if (passwordAddit == user.password)
                {
                    Session["User"] = user;
                    return View("FirstPage");
                }

                ViewBag.Message = "Incorrect password.";
                return View(model);
            }

            ViewBag.Message = "Podaci nisu validni.";
            return View(model);
        }

        
        //
        // GET: /Account/Register
        [AuthorizeGuest]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AuthorizeGuest]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userAddit = db.users.Where(u => u.mail == model.Email).FirstOrDefault();

                if (userAddit != null)
                {
                    ViewBag.Message = "This E-mail already exists.";
                    return View("Register");
                }

                var passwordAddit = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(model.Password);
                    byte[] hashBytes = md5Hash.ComputeHash(inputBytes);
                    
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    passwordAddit = sb.ToString();

                }


                User user = new User
                {
                    name = model.Firstname, mail = model.Email, lastname = model.Lastname, isAdmin = 0,
                    password = passwordAddit
                };
                
                db.users.Add(user);
                db.SaveChanges();

                return View("Login");
            }

            return View(model);
        }


        //
        // POST: /Account/LogOff
        [AuthorizeUserAndAdmin]
        public ActionResult LogOff()
        {
            Session["User"] = null;
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetNoStore();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        //   [Authorize]
        [AuthorizeUserAndAdmin]
        public ActionResult Edit()
        {
            return View();
        }
        
        [AuthorizeUserAndAdmin]
        public ActionResult AboutUser()
        {
            User user = Session["User"] as User;
            User user1 = db.users.Where(u => u.id == user.id).FirstOrDefault();
            return View(user1);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AuthorizeUserAndAdmin]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userAddit = Session["User"] as User;
                User userAddit1 = db.users.Where(u => u.id == userAddit.id).FirstOrDefault();

                var passwordAddit = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(model.Password);
                    byte[] hashBytes = md5Hash.ComputeHash(inputBytes);
                    
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    passwordAddit = sb.ToString();

                }

                userAddit.password = userAddit1.password = passwordAddit;
                userAddit.mail = userAddit1.mail = model.Email;
                userAddit.lastname = userAddit1.lastname = model.Lastname;
                userAddit.name = userAddit1.name = model.Firstname;
                db.Entry(userAddit1).State = EntityState.Modified;
                db.SaveChanges();
                return View("AboutUser", userAddit);
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