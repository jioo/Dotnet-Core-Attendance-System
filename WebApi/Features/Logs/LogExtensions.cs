using System;

namespace WebApi.Features.Logs
{
    public static class LogExtensions
    {
        public static string ToLocal(DateTime? utcDate) =>
            DateTime.Parse(utcDate.ToString()).ToLocalTime().ToString();

        public static DateTime ToLocalDateTime(DateTime? utcDate) =>
            DateTime.Parse(utcDate.ToString()).ToLocalTime();

        public static DateTime ToUtc(string localDate) =>
            DateTime.Parse(localDate).ToUniversalTime();
    }
}