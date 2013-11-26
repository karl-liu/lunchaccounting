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
    public class UserGroupController : ControllerBase
    {
        //
        // GET: /UserGroup/

        public ActionResult Index()
        {
            return View(db.UserGroups.ToList());
        }

        //
        // GET: /UserGroup/Details/5

        public ActionResult Details(int id = 0)
        {
            UserGroup usergroup = db.UserGroups.Find(id);
            if (usergroup == null)
            {
                return HttpNotFound();
            }
            return View(usergroup);
        }

        //
        // GET: /UserGroup/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserGroup/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserGroup usergroup)
        {
            if (ModelState.IsValid)
            {
                db.UserGroups.Add(usergroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usergroup);
        }

        //
        // GET: /UserGroup/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserGroup usergroup = db.UserGroups.Find(id);
            if (usergroup == null)
            {
                return HttpNotFound();
            }
            return View(usergroup);
        }

        //
        // POST: /UserGroup/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserGroup usergroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usergroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usergroup);
        }

        //
        // GET: /UserGroup/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserGroup usergroup = db.UserGroups.Find(id);
            if (usergroup == null)
            {
                return HttpNotFound();
            }
            return View(usergroup);
        }

        //
        // POST: /UserGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserGroup usergroup = db.UserGroups.Find(id);
            db.UserGroups.Remove(usergroup);
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