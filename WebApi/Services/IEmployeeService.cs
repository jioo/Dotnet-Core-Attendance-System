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
        Task<Employee> GetEmployeeByUserId(string id);
        Task AddAsync(Employee emp);
        Task<Employee> UpdateAsync(EmployeeViewModel emp);
        Task<bool> isCardExist(Guid id, string cardNo);
    }

}