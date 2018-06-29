using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface ILogService
    {
        Task<IList<LogViewModel>> GetAllAsync();
        Task<LogViewModel> FindAsync(Guid id);
        Task<Employee> CheckCardNo(LogInOutViewModel model);
        Task<LogResultViewModel> Log(Employee emp);
        Task<LogViewModel> UpdateAsync(LogEditViewModel model);
    }

}