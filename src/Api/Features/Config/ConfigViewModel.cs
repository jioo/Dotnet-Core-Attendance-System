using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Features.Config
{
    public class ConfigViewModel
    {
        public Guid Id { get; set; }
        
        [Required]
        public string TimeIn { get; set; }

        [Required]
        public string TimeOut { get; set; }

        [Required]
        [Range(0, 59)]
        public int GracePeriod { get; set; }
    }
}