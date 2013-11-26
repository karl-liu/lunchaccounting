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
    public class AppResourceAccessController : ControllerBase
    {
        //
        // GET: /AppResourceAccess/

        public ActionResult Index()
        {
            var appresourceaccess = db.AppResourceAccesses.Include(a => a.AppResource);

            foreach (var access in appresourceaccess)
            {
                if ("User".Equals(access.IdentityType, StringComparison.OrdinalIgnoreCase))
                {
                    var user = db.Users.Find(access.Identity_id);
                    if (user != null)
                    {
                        access.UserGroup_id = user.User_id;
                        access.IdentityName = user.UserName;
                    }
                }
                else if ("UserGroup".Equals(access.IdentityType, StringComparison.OrdinalIgnoreCase))
                {
                    var userGroup = db.UserGroups.Find(access.Identity_id);
                    if(userGroup!=null)
                    {
                        access.UserGroup_id = userGroup.UserGroup_id;
                        access.IdentityName = userGroup.GroupName;
                    }
                }
            }

            return View(appresourceaccess.ToList());
        }

        //
        // GET: /AppResourceAccess/Details/5

        public ActionResult Details(int id = 0)
        {
            AppResourceAccess appresourceaccess = db.AppResourceAccesses.Find(id);
            if (appresourceaccess == null)
            {
                return HttpNotFound();
            }
            return View(appresourceaccess);
        }

        //
        // GET: /AppResourceAccess/Create

        public ActionResult Create()
        {
            ViewBag.AppResource_id = new SelectList(db.AppResources, "AppResource_id", "Resource");
            ViewBag.IdentityType = new SelectList(IdentityTypeList);
            ViewBag.OperationType = new SelectList(OperationTypeList);
            ViewBag.User_id = new SelectList(GetUsersWithDefaultValue(), "User_id", "UserName");
            ViewBag.UserGroup_id = new SelectList(GetUserGroupsWithDefaultValue(), "UserGroup_id", "GroupName");            
            ViewBag.Access = new SelectList(AccessTypeList);

            return View();
        }

        //
        // POST: /AppResourceAccess/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppResourceAccess appresourceaccess)
        {
            if ("User".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.Identity_id = appresourceaccess.User_id;
            }
            else if ("UserGroup".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.Identity_id = appresourceaccess.UserGroup_id;
            }

            if (ModelState.IsValid)
            {
                db.AppResourceAccesses.Add(appresourceaccess);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppResource_id = new SelectList(db.AppResources, "AppResource_id", "Resource", appresourceaccess.AppResource_id);
            ViewBag.IdentityType = new SelectList(IdentityTypeList, appresourceaccess.IdentityType);
            ViewBag.OperationType = new SelectList(OperationTypeList, appresourceaccess.OperationType);
            ViewBag.User_id = new SelectList(GetUsersWithDefaultValue(), "User_id", "UserName", appresourceaccess.User_id);
            ViewBag.UserGroup_id = new SelectList(GetUserGroupsWithDefaultValue(), "UserGroup_id", "GroupName", appresourceaccess.UserGroup_id);
            ViewBag.Access = new SelectList(AccessTypeList);

            return View(appresourceaccess);
        }

        //
        // GET: /AppResourceAccess/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var appresourceaccess = db.AppResourceAccesses.Find(id);
            if (appresourceaccess == null)
            {
                return HttpNotFound();
            }

            if ("User".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.User_id = appresourceaccess.Identity_id;
            }
            else if ("UserGroup".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.UserGroup_id = appresourceaccess.Identity_id;
            }

            ViewBag.AppResource_id = new SelectList(db.AppResources, "AppResource_id", "Resource", appresourceaccess.AppResource_id);
            ViewBag.IdentityType = new SelectList(IdentityTypeList, appresourceaccess.IdentityType);
            ViewBag.OperationType = new SelectList(OperationTypeList, appresourceaccess.OperationType);
            ViewBag.User_id = new SelectList(GetUsersWithDefaultValue(), "User_id", "UserName", appresourceaccess.User_id);
            ViewBag.UserGroup_id = new SelectList(GetUserGroupsWithDefaultValue(), "UserGroup_id", "GroupName", appresourceaccess.UserGroup_id);
            ViewBag.Access = new SelectList(AccessTypeList, appresourceaccess.Access);

            return View(appresourceaccess);
        }

        //
        // POST: /AppResourceAccess/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppResourceAccess appresourceaccess)
        {
            if ("User".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.Identity_id = appresourceaccess.User_id;
            }
            else if ("UserGroup".Equals(appresourceaccess.IdentityType, StringComparison.OrdinalIgnoreCase))
            {
                appresourceaccess.Identity_id = appresourceaccess.UserGroup_id;
            }

            if (ModelState.IsValid)
            {
                db.Entry(appresourceaccess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppResource_id = new SelectList(db.AppResources, "AppResource_id", "Resource", appresourceaccess.AppResource_id);
            ViewBag.IdentityType = new SelectList(IdentityTypeList, appresourceaccess.IdentityType);
            ViewBag.OperationType = new SelectList(OperationTypeList, appresourceaccess.OperationType);
            ViewBag.User_id = new SelectList(GetUsersWithDefaultValue(), "User_id", "UserName", appresourceaccess.User_id);
            ViewBag.UserGroup_id = new SelectList(GetUserGroupsWithDefaultValue(), "UserGroup_id", "GroupName", appresourceaccess.UserGroup_id);
            ViewBag.Access = new SelectList(AccessTypeList, appresourceaccess.Access);

            return View(appresourceaccess);
        }

        //
        // GET: /AppResourceAccess/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AppResourceAccess appresourceaccess = db.AppResourceAccesses.Find(id);
            if (appresourceaccess == null)
            {
                return HttpNotFound();
            }
            return View(appresourceaccess);
        }

        //
        // POST: /AppResourceAccess/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppResourceAccess appresourceaccess = db.AppResourceAccesses.Find(id);
            db.AppResourceAccesses.Remove(appresourceaccess);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region private methods

        private List<UserGroup> GetUserGroupsWithDefaultValue()
        {
            var groups = new List<UserGroup>();
            groups.Add(new UserGroup() { UserGroup_id = -1, GroupName = string.Empty });
            groups.AddRange(db.UserGroups.ToList());
            return groups;
        }

        private List<Models.User> GetUsersWithDefaultValue()
        {
            List<User> users = new List<User>();
            users.Add(new User() { User_id = -1, UserName = "" });
            users.AddRange(db.Users.ToList());
            return users;
        }

        #endregion

    }
}