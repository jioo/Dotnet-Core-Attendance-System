using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using WebApi.Entities;
using WebApi.ViewModels;
using WebApi.Repositories;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class LogService : ILogService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Log> _repoLog;
        private readonly IRepository<Employee> _repoEmp;
        private readonly UserManager<User> _manager;

        public LogService(
            IMapper mapper,
            IRepository<Log> repoLog,
            IRepository<Employee> repoEmp,
            UserManager<User> manager)
        {
            _mapper = mapper;
            _repoLog = repoLog;
            _repoEmp = repoEmp;
            _manager = manager;
        }

        /// <summary>
        /// <see cref="ILogService.LogViewModel"/>
        /// </summary>
        public async Task<IList<LogViewModel>> GetAllAsync()
        {
            try
            {
                return await _repoLog.Context.Query()
                    .Where(m => m.Deleted == null)
                    .OrderByDescending(m => m.Created)
                    .Include(m => m.Employee)
                    .Select(m => new LogViewModel
                    {
                        Id = m.Id,
                        EmployeeId = m.EmployeeId,
                        TimeIn = DateHelper.ToLocal(m.TimeIn),
                        TimeOut = (m.TimeOut == null) ? "": DateHelper.ToLocal(m.TimeOut),
                        Created = m.Created, 
                        Updated = m.Updated,
                        Deleted = m.Deleted,
                        FullName = m.Employee.FullName
                    })
                    .ToListAsync();
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ILogService.FindAsync"/>
        /// </summary>
        public async Task<LogViewModel> FindAsync(Guid id)
        {
            try
            {
                return await _repoLog.Context.Query()
                    .Where(m => m.Id == id)
                    .Include(m => m.Employee)
                    .Select(m => new LogViewModel
                    {
                        Id = m.Id,
                        EmployeeId = m.EmployeeId,
                        TimeIn = DateHelper.ToLocal(m.TimeIn),
                        TimeOut = (m.TimeOut == null) ? "": DateHelper.ToLocal(m.TimeOut),
                        Created = m.Created, 
                        Updated = m.Updated,
                        Deleted = m.Deleted,
                        FullName = m.Employee.FullName
                    })
                    .FirstAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ILogService.ValidateTimeInOutCredentials"/>
        /// </summary>
        public async Task<Employee> ValidateTimeInOutCredentials(LogInOutViewModel model)
        {
            try
            {
                // Get employee information
                var emp = await _repoEmp.Context.Query()
                    .Where(m => m.CardNo == model.CardNo)
                    .Where(m => m.Status == Status.Active)
                    .Where(m => m.Deleted == null)
                    .SingleOrDefaultAsync();
                
                // Check if employee exist
                if(emp == null) return new Employee{ Id = Guid.Empty };

                // Check if password is correct
                var user = await _manager.FindByIdAsync(emp.IdentityId);
                if (!await _manager.CheckPasswordAsync(user, model.Password))
                    return new Employee{ Id = Guid.Empty };

                return emp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ILogService.Log"/>
        /// </summary>
        public async Task<LogResultViewModel> Log(Employee employee)
        {
            try
            {
                var newLog = new Log();
                var log = await _repoLog.Context.Query()
                    .Where(m => m.EmployeeId == employee.Id)
                    .Where(m => m.TimeOut == null)
                    .Where(m => m.Deleted == null)
                    .SingleOrDefaultAsync();

                // Log in user
                if (log == null)
                {
                    newLog.EmployeeId = employee.Id;
                    newLog.TimeIn = DateTime.UtcNow;
                    _repoLog.Context.Insert(newLog);
                }
                // Log out user
                else
                {
                    var oldModel = _repoLog.Context.GetById(log.Id);
                    _mapper.Map(log, oldModel);
                    oldModel.TimeOut = DateTime.UtcNow;

                    _repoLog.Context.Update(oldModel);
                }

                await _repoLog.SaveAsync();
                var result = (log != null) ? log : newLog;

                return new LogResultViewModel
                {   
                    FullName = result.Employee.FullName,
                    CardNo = employee.CardNo,
                    Position = employee.Position,
                    TimeIn = DateHelper.ToLocal(result.TimeIn),
                    TimeOut = (result.TimeOut != null) 
                        ? DateHelper.ToLocal(result.TimeOut)
                        : string.Empty
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="ILogService.UpdateAsync"/>
        /// </summary>
        public async Task<LogViewModel> UpdateAsync(LogEditViewModel viewModel)
        {
            try
            {
                var model = _repoLog.Context.GetById(viewModel.Id);
                // Convert datetime to UTC before updating
                viewModel.TimeIn = DateHelper.ToUtc(viewModel.TimeIn.ToString());
                viewModel.TimeOut =  (viewModel.TimeOut != null) 
                    ? DateHelper.ToUtc(viewModel.TimeOut.ToString())
                    : viewModel.TimeOut;
                
                _mapper.Map(viewModel, model);
                model.Updated = DateTime.UtcNow;

                _repoLog.Context.Update(model);
                await _repoLog.SaveAsync();

                return await FindAsync(model.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}