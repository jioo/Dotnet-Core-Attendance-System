using System;
using WebApi.Entities;

namespace Test.Api.Models
{
    public class EmployeeTestModel
    {
        public Guid Id { get; set; } = new Guid("718cae62-3856-4620-bada-321cc66023f0");
        public string UserName { get; set; } = "jioo";
        public string Password { get; set; } = "123456";
        public string FullName { get; set; } = "Justine Joshua Quiazon";
        public string CardNo { get; set; } = "123456";
        public string Position { get; set; } = "Software Developer";
    }
}