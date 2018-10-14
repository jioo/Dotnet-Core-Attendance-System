using System;

namespace WebApi.Features.Config
{
    public class ConfigViewModel
    {
        public Guid Id { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string GracePeriod { get; set; }
    }
}