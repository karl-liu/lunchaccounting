using LunchAccounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LunchAccounting.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            var today = Utilities.ToDateWithTimeZone(DateTime.Now.ToUniversalTime(), Utilities.ChinaTimeZoneId);
            var todayRegister = db.LunchRegisters.Where(p => p.User_id == CurrentUser.User_id && p.LunchDate == today.Date);

            if (todayRegister.Count() < 1)
            {
                ViewBag.Action = "Lunchregister/create";
                ViewBag.Message = "Want to have a Lunch today"; 
            }
            else if (todayRegister.Count() == 1)
            {
                ViewBag.Action = "consume/create";
                ViewBag.Message = "Please submit your today's consume!"; 
            }
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact List.";

            return View();
        }
    }
}
