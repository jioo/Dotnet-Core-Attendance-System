using System;

namespace WebApi.Features.Configurations
{
    public class ConfigurationViewModel
    {
        public Guid Id { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string GracePeriod { get; set; }
    }
}