using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Features.Logs
{
    public class LogEditViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}