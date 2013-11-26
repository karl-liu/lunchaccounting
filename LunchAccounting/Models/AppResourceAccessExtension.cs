using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LunchAccounting.Models
{
    public partial class AppResourceAccess
    {
        public int User_id { get; set; }
        public int UserGroup_id { get; set; }
        public string IdentityName { get; set; }
    }
}