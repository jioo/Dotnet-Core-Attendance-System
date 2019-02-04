using System.Collections.Generic;
using WebApi.Entities;
using WebApi.Features.Logs;

namespace Test.Api
{
    public class LogListResponseModel
    {
        public DateFilteredList Meta  { get; set; }
        public ICollection<LogViewModel> Data { get; set; }
    }
}