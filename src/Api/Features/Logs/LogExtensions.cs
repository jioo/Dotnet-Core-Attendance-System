using System;
using System.Linq;
using WebApi.Entities;

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

        public static IQueryable<LogViewModel> MapToViewModel(this IQueryable<Log> logs) =>
            logs.Select(m => new LogViewModel
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                TimeIn = LogExtensions.ToLocal(m.TimeIn),
                TimeOut = (m.TimeOut == null) ? "": LogExtensions.ToLocal(m.TimeOut),
                Created = m.Created, 
                Updated = m.Updated,
                Deleted = m.Deleted,
                FullName = m.Employee.FullName
            });
    }
}