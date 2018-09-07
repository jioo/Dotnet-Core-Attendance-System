using System;

namespace WebApi.Helpers
{
    public class DateHelper 
    {
        public static string ToLocal(DateTime? utcDate) =>
            DateTime.Parse(utcDate.ToString()).ToLocalTime().ToString();

        public static DateTime ToLocalDateTime(DateTime? utcDate) =>
            DateTime.Parse(utcDate.ToString()).ToLocalTime();

        public static DateTime ToUtc(string localDate) =>
            DateTime.Parse(localDate).ToUniversalTime();
    }
}