using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.ViewModels
{
    public class EmployeeViewModel : BaseEntity
    {
        [Required]
        public string IdentityId { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Position { get; set; }
        [Required]
        public string CardNo { get; set; }
        public Status Status { get; set; }

        public virtual User Identity { get; set; }
    }
}