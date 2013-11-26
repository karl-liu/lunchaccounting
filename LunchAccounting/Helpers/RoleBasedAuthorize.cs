using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace LunchAccounting.Helpers
{
    public class RoleBasedAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isInRole = false;
            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var roles = Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var db = new LunchAccounting.Models.LunchAccountingEntities();

                var users = db.Users.Include(a => a.UserGroupMembers);
                var user = users.Where(p => p.UserName == httpContext.User.Identity.Name).FirstOrDefault();
                
                foreach (var role in roles)
                {
                    foreach (var userGroupMember in user.UserGroupMembers)
                    {
                        if (userGroupMember.UserGroup.GroupName.Equals(role, StringComparison.OrdinalIgnoreCase))
                        {
                            isInRole = true;
                            break;
                        }
                    }

                    if (isInRole)
                    {
                        break;
                    }
                }                
            }

            return isInRole;
        }
    }
}