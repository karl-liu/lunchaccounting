using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LunchAccounting.Models;
using LunchAccounting.Helpers;

namespace LunchAccounting.Controllers
{
    public class LunchRegisterController : Controller
    {
        private LunchAccountingEntities db = new LunchAccountingEntities();

        public ActionResult TodayList()
        {
            var today = Utilities.ToDateWithTimeZone(DateTime.Now.ToUniversalTime(), Utilities.ChinaTimeZoneId);

            var Lunchregisters = db.LunchRegisters
                .Include(l => l.User)
                .Where(p => p.LunchDate == today.Date);

            return View(Lunchregisters.ToList());
        }

        //
        // GET: /LunchRegister/

        public ActionResult Index()
        {
            var today = Utilities.ToDateWithTimeZone(DateTime.Now.ToUniversalTime(), Utilities.ChinaTimeZoneId);

            var Lunchregisters = db.LunchRegisters
                .Include(l => l.User)
                .Include(l => l.UserConsumes)
                .Where(p => p.LunchDate == today.Date &&
                       p.User.UserName.Equals(HttpContext.User.Identity.Name));

            foreach (var register in Lunchregisters)
            {
                register.TotalConsume = register.UserConsumes.Sum(p => p.Amount);
            }

             return View(Lunchregisters.ToList());
        }

        //
        // GET: /LunchRegister/Details/5

        public ActionResult Details(int id = 0)
        {
            LunchRegister Lunchregister = db.LunchRegisters.Find(id);
            if (Lunchregister == null)
            {
                return HttpNotFound();
            }
            return View(Lunchregister);
        }

        //
        // GET: /LunchRegister/Create

        public ActionResult Create()
        {
            var user = db.Users.Where(p => p.UserName == HttpContext.User.Identity.Name).FirstOrDefault();
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", user.User_id);
            return View();
        }

        //
        // POST: /LunchRegister/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LunchRegister Lunchregister)
        {
            Lunchregister.LunchDate = Utilities.ToDateWithTimeZone(DateTime.Now.ToUniversalTime(), Utilities.ChinaTimeZoneId).Date;

            if (ModelState.IsValid)
            {
                db.LunchRegisters.Add(Lunchregister);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", Lunchregister.User_id);
            return View(Lunchregister);
        }

        //
        // GET: /LunchRegister/Edit/5

        public ActionResult Edit(int id = 0)
        {
            LunchRegister Lunchregister = db.LunchRegisters.Find(id);
            if (Lunchregister == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", Lunchregister.User_id);
            return View(Lunchregister);
        }

        //
        // POST: /LunchRegister/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LunchRegister Lunchregister)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Lunchregister).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", Lunchregister.User_id);
            return View(Lunchregister);
        }

        //
        // GET: /LunchRegister/Delete/5

        public ActionResult Delete(int id = 0)
        {
            LunchRegister Lunchregister = db.LunchRegisters.Find(id);
            if (Lunchregister == null)
            {
                return HttpNotFound();
            }
            return View(Lunchregister);
        }

        //
        // POST: /LunchRegister/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LunchRegister Lunchregister = db.LunchRegisters.Find(id);
            db.LunchRegisters.Remove(Lunchregister);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}