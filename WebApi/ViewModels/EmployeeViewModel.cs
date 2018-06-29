using System;
using WebApi.Entities;

namespace WebApi.ViewModels
{
    public class EmployeeViewModel : BaseEntity
    {
        public string IdentityId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string CardNo { get; set; }
        public Status Status { get; set; }
    }
}