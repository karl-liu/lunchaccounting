using LunchAccounting.Helpers;
using LunchAccounting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchAccounting.Controllers
{
    public class ControllerBase : Controller
    {
        protected User CurrentUser
        {
            get
            {
                var currentUserName = HttpContext.User.Identity.Name;
                var user = db.Users.Where(p => p.UserName == currentUserName).First();
                return user;
            }
        }

        protected LunchAccounting.Models.LunchAccountingEntities db = new LunchAccounting.Models.LunchAccountingEntities();

        protected static List<string> IdentityTypeList = new List<string>() {
            "User", "UserGroup" 
        };

        protected static List<string> OperationTypeList = new List<string>() { 
            "View", "Edit", "Delete", "Create" 
        };

        protected static List<string> AccessTypeList = new List<string>() { 
            "allow", "deny"
        };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!("user".Equals(filterContext.RouteData.Values["controller"].ToString(), StringComparison.InvariantCultureIgnoreCase) &&
                "create".Equals(filterContext.RouteData.Values["action"].ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {


                var currentUserName = HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(currentUserName))
                {
                    var currentUserData = db.Users.Where(p => p.UserName == currentUserName);
                    if (currentUserData.Count() == 0)
                    {
                        filterContext.Result = RedirectToAction("create", "user");
                    }
                }
                else
                {
                    filterContext.Result = RedirectToAction("create", "user");
                }
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = new HandleErrorInfo(
                filterContext.Exception, 
                filterContext.RouteData.Values["controller"].ToString(), 
                filterContext.RouteData.Values["controller"].ToString());
            View("Error", error).ExecuteResult(ControllerContext);
        }

        protected bool IsCurrentUser(string userName)
        {
            return !string.IsNullOrEmpty(userName) && userName.Equals(HttpContext.User.Identity.Name);
        }
    }
}