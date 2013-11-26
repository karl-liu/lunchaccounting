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
    public class UserGroupMemberController : ControllerBase
    {
        //
        // GET: /UserGroupMember/

        public ActionResult Index()
        {
            var usergroupmember = db.UserGroupMembers.Include(u => u.User).Include(u => u.UserGroup);
            return View(usergroupmember.ToList());
        }

        //
        // GET: /UserGroupMember/Details/5

        public ActionResult Details(int id = 0)
        {
            UserGroupMember usergroupmember = db.UserGroupMembers.Find(id);
            if (usergroupmember == null)
            {
                return HttpNotFound();
            }
            return View(usergroupmember);
        }

        //
        // GET: /UserGroupMember/Create

        public ActionResult Create()
        {
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName");
            ViewBag.UserGroup_id = new SelectList(db.UserGroups, "UserGroup_id", "GroupName");
            return View();
        }

        //
        // POST: /UserGroupMember/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserGroupMember usergroupmember)
        {
            if (ModelState.IsValid)
            {
                db.UserGroupMembers.Add(usergroupmember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", usergroupmember.User_id);
            ViewBag.UserGroup_id = new SelectList(db.UserGroups, "UserGroup_id", "GroupName", usergroupmember.UserGroup_id);
            return View(usergroupmember);
        }

        //
        // GET: /UserGroupMember/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserGroupMember usergroupmember = db.UserGroupMembers.Find(id);
            if (usergroupmember == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", usergroupmember.User_id);
            ViewBag.UserGroup_id = new SelectList(db.UserGroups, "UserGroup_id", "GroupName", usergroupmember.UserGroup_id);
            return View(usergroupmember);
        }

        //
        // POST: /UserGroupMember/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserGroupMember usergroupmember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usergroupmember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", usergroupmember.User_id);
            ViewBag.UserGroup_id = new SelectList(db.UserGroups, "UserGroup_id", "GroupName", usergroupmember.UserGroup_id);
            return View(usergroupmember);
        }

        //
        // GET: /UserGroupMember/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserGroupMember usergroupmember = db.UserGroupMembers.Find(id);
            if (usergroupmember == null)
            {
                return HttpNotFound();
            }
            return View(usergroupmember);
        }

        //
        // POST: /UserGroupMember/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserGroupMember usergroupmember = db.UserGroupMembers.Find(id);
            db.UserGroupMembers.Remove(usergroupmember);
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