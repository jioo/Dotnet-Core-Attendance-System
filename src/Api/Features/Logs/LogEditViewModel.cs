using System;
using WebApi.Entities;

namespace WebApi.Features.Logs
{
    public class LogEditViewModel : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public string FullName { get; set; }
    }
}