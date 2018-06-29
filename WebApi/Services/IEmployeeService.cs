using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface IEmployeeService
    {
        Task<IList<Employee>> GetAllAsync();
        Task<Employee> FindAsync(Guid id);
        Task AddAsync(Employee emp);
        Task UpdateAsync(EmployeeViewModel emp);
        Task<bool> isCardExist(Guid id, string cardNo);
    }

}