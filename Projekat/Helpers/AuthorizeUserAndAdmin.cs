using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Projekat.Helpers
{
    public class AuthorizeUserAndAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = filterContext.HttpContext.Session["User"] as User;

            if (user == null)
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("action", "Login");
                redirectTargetDictionary.Add("controller", "Account");
                redirectTargetDictionary.Add("area", "");

                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}