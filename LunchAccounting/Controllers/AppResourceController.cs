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
    [RoleBasedAuthorize(Roles="admin")]
    public class AppResourceController : ControllerBase
    {

        //
        // GET: /AppResource/

        public ActionResult Index()
        {
            return View(db.AppResources.ToList());
        }

        //
        // GET: /AppResource/Details/5

        public ActionResult Details(int id = 0)
        {
            AppResource appresource = db.AppResources.Find(id);
            if (appresource == null)
            {
                return HttpNotFound();
            }
            return View(appresource);
        }

        //
        // GET: /AppResource/Create

        public ActionResult Create()
        {
            ViewBag.DefaultAccess = new SelectList(AccessTypeList, "deny");
            return View();
        }

        //
        // POST: /AppResource/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppResource appresource)
        {
            if (ModelState.IsValid)
            {
                db.AppResources.Add(appresource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DefaultAccess = new SelectList(AccessTypeList, "deny");
            return View(appresource);
        }

        //
        // GET: /AppResource/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AppResource appresource = db.AppResources.Find(id);
            if (appresource == null)
            {
                return HttpNotFound();
            }

            ViewBag.DefaultAccess = new SelectList(AccessTypeList, appresource.DefaultAccess);
            return View(appresource);
        }

        //
        // POST: /AppResource/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppResource appresource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appresource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DefaultAccess = new SelectList(AccessTypeList, appresource.DefaultAccess);
            return View(appresource);
        }

        //
        // GET: /AppResource/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AppResource appresource = db.AppResources.Find(id);
            if (appresource == null)
            {
                return HttpNotFound();
            }
            return View(appresource);
        }

        //
        // POST: /AppResource/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppResource appresource = db.AppResources.Find(id);
            db.AppResources.Remove(appresource);
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