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

        public async Task<IList<LogViewModel>> GetAllAsync()
        {
            // return await _repoLog.Context.GetAllAsync();
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

        public async Task<LogViewModel> FindAsync(Guid id)
        {
            // return await _repoLog.Context.GetByIdAsync(id);
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

        public async Task<Employee> CheckCardNo(LogInOutViewModel model)
        {
            try
            {
                var emp = await _repoEmp.Context.Query()
                    .Where(m => m.CardNo == model.CardNo)
                    .Where(m => m.Status == Status.Active)
                    .Where(m => m.Deleted == null)
                    .SingleOrDefaultAsync();
                    
                if(emp == null) 
                {
                    return new Employee{ Id = Guid.Empty };
                }

                var user = await _manager.FindByIdAsync(emp.IdentityId);
                if (await _manager.CheckPasswordAsync(user, model.Password))
                {
                    return emp;
                }
                else
                {
                    return new Employee{ Id = Guid.Empty };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LogResultViewModel> Log(Employee emp)
        {
            try
            {
                var newLog = new Log();
                var log = await _repoLog.Context.Query()
                    .Where(m => m.EmployeeId == emp.Id)
                    .Where(m => m.TimeOut == null)
                    .Where(m => m.Deleted == null)
                    .SingleOrDefaultAsync();

                // Log in user
                if (log == null)
                {
                    newLog.EmployeeId = emp.Id;
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
                    CardNo = emp.CardNo,
                    Position = emp.Position,
                    TimeIn = DateHelper.ToLocal(result.TimeIn),
                    TimeOut = (result.TimeOut == null) ? "": DateHelper.ToLocal(result.TimeOut)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LogViewModel> UpdateAsync(LogEditViewModel model)
        {
            try
            {
                var oldModel = _repoLog.Context.GetById(model.Id);
                model.TimeIn = DateHelper.ToUtc(model.TimeIn.ToString());
                model.TimeOut =  (model.TimeOut != null) ? DateHelper.ToUtc(model.TimeOut.ToString()): model.TimeOut;
                _mapper.Map(model, oldModel);
                oldModel.Updated = DateTime.UtcNow;

                _repoLog.Context.Update(oldModel);
                await _repoLog.SaveAsync();

                return await FindAsync(oldModel.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}