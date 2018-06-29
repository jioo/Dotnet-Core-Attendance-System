using System;
using WebApi.Entities;

namespace WebApi.ViewModels
{
    public class LogResultViewModel 
    {
        public string FullName { get; set; }
        public string CardNo { get; set; }
        public string Position { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
    }
}