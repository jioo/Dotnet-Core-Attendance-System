using WebApi.Entities;
using AutoMapper;

namespace WebApi.Features.Employees
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Entities.Employee, EmployeeViewModel>();
            CreateMap<EmployeeViewModel, Entities.Employee>();
        }
    }
}