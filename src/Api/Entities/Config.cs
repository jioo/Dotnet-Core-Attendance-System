using System;

namespace WebApi.Entities
{
    public class Config
    {
        public Guid Id { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public int GracePeriod { get; set; }
    }
}