using System;

namespace WebApi.Entities
{
    public class Log : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }

        public virtual Employee Employee { get; set; }
    }
}