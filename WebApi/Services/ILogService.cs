using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface ILogService
    {
        /// <summary>
        /// List of logs
        /// </summary>
        Task<IList<LogViewModel>> GetAllAsync();

        /// <summary>
        /// Find specific log
        /// </summary>
        /// <param name="id">id to search for</param>
        /// <returns>
        /// Returns <see cref="LogViewModel"/>
        /// </returns>
        Task<LogViewModel> FindAsync(Guid id);

        /// <summary>
        /// Validate employee Time In/Out
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>
        /// Returns <see cref="Employee"/>
        /// </returns>
        Task<Employee> ValidateTimeInOutCredentials(LogInOutViewModel viewModel);

        /// <summary>
        /// Log the time of employee time In/Out
        /// </summary>
        /// <param name="employee">employee model</param>
        /// <returns>
        /// Returns <see cref="LogResultViewModel"/>
        /// </returns>
        Task<LogResultViewModel> Log(Employee employee);

        /// <summary>
        /// Update existing log
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>
        /// Returns <see cref="LogViewModel"/>
        /// </returns>
        Task<LogViewModel> UpdateAsync(LogEditViewModel viewModel);
    }

}