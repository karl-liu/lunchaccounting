using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;

namespace LunchAccounting.Helpers
{
    public class ResourceBasedAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool hasAccess = false;

            var db = new LunchAccounting.Models.LunchAccountingEntities();
            var user = db.Users
                .Include(a => a.UserGroupMembers)
                .Where(p => p.UserName == httpContext.User.Identity.Name)
                .FirstOrDefault();

            var resource = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            var operation = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();

            var userAccesses = db.AppResourceAccesses.Include(p => p.AppResource)
                .Where(
                    p => "User".Equals(p.IdentityType, StringComparison.OrdinalIgnoreCase) &&
                    p.Identity_id == user.User_id &&
                    resource.Equals(p.AppResource.Resource, StringComparison.OrdinalIgnoreCase) &&
                    (p.OperationType == null || p.OperationType == string.Empty || operation.Equals(p.OperationType, StringComparison.OrdinalIgnoreCase))
                );

            var userGroupIds = user.UserGroupMembers.Select(a => a.UserGroup_id).ToList();
            var groupAccesses = db.AppResourceAccesses
                .Where(p => "UserGroup".Equals(
                    p.IdentityType, StringComparison.OrdinalIgnoreCase) &&
                    userGroupIds.Contains(p.Identity_id) &&
                    resource.Equals(p.AppResource.Resource, StringComparison.OrdinalIgnoreCase) &&
                    (p.OperationType == null || p.OperationType == string.Empty || operation.Equals(p.OperationType, StringComparison.OrdinalIgnoreCase))
                );

            if (userAccesses.Count() == 0 && groupAccesses.Count() == 0)
            {
                var resourceInfo = db.AppResources.Where(p => resource.Equals(p.Resource, StringComparison.OrdinalIgnoreCase));
                return resourceInfo.Count() == 1 && "allow".Equals(resourceInfo.First().DefaultAccess, StringComparison.OrdinalIgnoreCase);
            }

            foreach (var useraccess in userAccesses)
            {
                if ("deny".Equals(useraccess.Access, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if ("allow".Equals(useraccess.Access, StringComparison.OrdinalIgnoreCase))
                {
                    hasAccess = true;
                }
            }

            foreach (var groupaccess in groupAccesses)
            {
                if ("deny".Equals(groupaccess.Access, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if ("allow".Equals(groupaccess.Access, StringComparison.OrdinalIgnoreCase))
                {
                    hasAccess = true;
                }
            }

            return hasAccess;
        }
    }
}