using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LunchAccounting.Helpers
{
    public static class Utilities
    {
        public const string ChinaTimeZoneId = "China Standard Time";

        public static DateTime ToDateWithTimeZone(DateTime utcDateTime, string timeZoneId)
        {
            return TimeZoneInfo.ConvertTime(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)); 
        }
    }
}