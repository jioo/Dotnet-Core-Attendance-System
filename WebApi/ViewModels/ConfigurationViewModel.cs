using System;
using WebApi.Entities;

namespace WebApi.ViewModels
{
    public class ConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string GracePeriod { get; set; }
    }
}