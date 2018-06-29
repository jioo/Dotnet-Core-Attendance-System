using System;
using WebApi.Entities;

namespace WebApi.ViewModels
{
    public class LogViewModel : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string FullName { get; set; }
    }
}