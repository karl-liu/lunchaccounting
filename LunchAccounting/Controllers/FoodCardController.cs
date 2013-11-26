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
    [RoleBasedAuthorize(Roles = "admin")]
    public class FoodCardController : ControllerBase
    {
        //
        // GET: /FoodCard/

        public ActionResult Index()
        {
            var foodcards = db.FoodCards.Include(f => f.OwnerInfo);
            return View(foodcards.ToList());
        }

        //
        // GET: /FoodCard/Details/5

        public ActionResult Details(int id = 0)
        {
            FoodCard foodcard = db.FoodCards.Find(id);
            if (foodcard == null)
            {
                return HttpNotFound();
            }
            return View(foodcard);
        }

        //
        // GET: /FoodCard/Create

        public ActionResult Create()
        {
            ViewBag.Owner_id = new SelectList(db.Users, "User_id", "UserName");
            return View();
        }

        //
        // POST: /FoodCard/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FoodCard foodcard)
        {
            if (ModelState.IsValid)
            {
                foodcard.CreateDate = DateTime.Now;

                db.FoodCards.Add(foodcard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Owner_id = new SelectList(db.Users, "User_id", "UserName", foodcard.Owner_id);
            return View(foodcard);
        }

        //
        // GET: /FoodCard/Edit/5

        public ActionResult Edit(int id = 0)
        {
            FoodCard foodcard = db.FoodCards.Find(id);
            if (foodcard == null)
            {
                return HttpNotFound();
            }
            ViewBag.Owner_id = new SelectList(db.Users, "User_id", "UserName", foodcard.Owner_id);
            return View(foodcard);
        }

        //
        // POST: /FoodCard/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodCard foodcard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foodcard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Owner_id = new SelectList(db.Users, "User_id", "UserName", foodcard.Owner_id);
            return View(foodcard);
        }

        //
        // GET: /FoodCard/Delete/5

        public ActionResult Delete(int id = 0)
        {
            FoodCard foodcard = db.FoodCards.Find(id);
            if (foodcard == null)
            {
                return HttpNotFound();
            }
            return View(foodcard);
        }

        //
        // POST: /FoodCard/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodCard foodcard = db.FoodCards.Find(id);
            db.FoodCards.Remove(foodcard);
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