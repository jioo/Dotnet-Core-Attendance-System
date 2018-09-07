using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using WebApi.Entities;
using WebApi.ViewModels;
using WebApi.Repositories;
using WebApi.Infrastructures;

namespace WebApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly IRepository<Employee> _repo;
        private readonly ApplicationDbContext _context;

        public EmployeeService(
            UserManager<User> manager, 
            IMapper mapper, IRepository<Employee> repo, 
            ApplicationDbContext context)
        {
            _manager = manager;
            _mapper = mapper;
            _repo = repo;
            _context = context;
        }

        /// <summary>
        /// <see cref="IEmployeeService.GetAllAsync"/>
        /// </summary>
        public async Task<IList<Employee>> GetAllAsync()
        {
            try
            {
                return await _repo.Context.Query()
                    .OrderByDescending(m => m.Created)
                    .Include(m => m.Identity)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IEmployeeService.FindAsync"/>
        /// </summary>
        public async Task<EmployeeViewModel> FindAsync(Guid id)
        {
            try
            {
                var model =  await _repo.Context.GetByIdAsync(id);
                return _mapper.Map<EmployeeViewModel>(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IEmployeeService.isCardExist"/>
        /// </summary>
        public async Task<bool> isCardExist(Guid id, string cardNo)
        {
            try
            {
                var res = await _repo.Context.Query()
                    .Where(m => m.Id != id)
                    .Where(m => m.CardNo == cardNo)
                    .Where(m => m.Deleted == null)
                    .CountAsync();
                return (res > 0) ? true: false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IEmployeeService.GetEmployeeByUserId"/>
        /// </summary>
        public async Task<EmployeeViewModel> GetEmployeeByUserId(string id)
        {
            try
            {
                var result = await _repo.Context.Query()
                    .Where(m => m.IdentityId == id)
                    .FirstOrDefaultAsync();

                return (result != null) 
                    ?  _mapper.Map<EmployeeViewModel>(result)
                    :  new EmployeeViewModel{ FullName = "Administrator" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IEmployeeService.AddAsync"/>
        /// </summary>
        public async Task<EmployeeViewModel> AddAsync(EmployeeViewModel viewModel)
        {
            try
            {
                var model = _mapper.Map<Employee>(viewModel);
                _repo.Context.Insert(model);
                await _repo.SaveAsync();
                
                return _mapper.Map<EmployeeViewModel>(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// <see cref="IEmployeeService.UpdateAsync"/>
        /// </summary>
        public async Task<EmployeeViewModel> UpdateAsync(EmployeeViewModel viewModel)
        {
            try
            {
                var model = _repo.Context.GetById(viewModel.Id);
                _mapper.Map(viewModel, model);
                model.Updated = DateTime.UtcNow;

                _repo.Context.Update(model);
                await _repo.SaveAsync();

                return _mapper.Map<EmployeeViewModel>(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}