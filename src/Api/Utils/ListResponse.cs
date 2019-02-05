using System.Collections.Generic;
using WebApi.Entities;

namespace WebApi.Utils
{
    public class ListResponse<T> where T : class
    {
        public DateFilteredList Meta  { get; set; }
        public ICollection<T> Data { get; set; }
    }
}