using System.Collections.Generic;

namespace WebApi.Entities
{
    public class Employee : BaseEntity
    {
        public string IdentityId { get; set; }
        public User Identity { get; set; }  // navigation property
        public string FullName { get; set; }
        public string Position { get; set; }
        public string CardNo { get; set; }
        public Status Status { get; set; } = Status.Active;

        public virtual ICollection<Log> Logs { get; set; }
    }

    public enum Status
    {
        Inactive,
        Active
    }
}