using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface IEmployeeService
    {
        /// <summary>
        /// List of employees
        /// </summary>
        Task<IList<Employee>> GetAllAsync();

        /// <summary>
        /// Find an employee
        /// </summary>
        /// <param name="id">id to search for</param>
        /// <returns>
        /// Returns <see cref="EmployeeViewModel"/>
        /// </returns>
        Task<EmployeeViewModel> FindAsync(Guid id);

        /// <summary>
        /// Find an employee using Identity id
        /// </summary>
        /// <param name="id">id to search for</param>
        /// <returns>
        /// Returns <see cref="Employee"/>
        /// </returns>
        Task<EmployeeViewModel> GetEmployeeByUserId(string id);

        /// <summary>
        /// Insert new employee
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>
        /// Returns <see cref="EmployeeViewModel"/>
        /// </returns>
        Task<EmployeeViewModel> AddAsync(EmployeeViewModel viewModel);

        /// <summary>
        /// Update existing employee
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>
        /// Returns <see cref="EmployeeViewModel"/>
        /// </returns>
        Task<EmployeeViewModel> UpdateAsync(EmployeeViewModel viewModel);

        /// <summary>
        /// Checks if Card No already exist
        /// </summary>
        /// <param name="id">id to check for</param>
        /// <param name="cardNo">card no. to check for</param>
        /// <returns>
        /// Returns <see cref="bool"/>
        /// </returns>
        Task<bool> isCardExist(Guid id, string cardNo);
    }

}