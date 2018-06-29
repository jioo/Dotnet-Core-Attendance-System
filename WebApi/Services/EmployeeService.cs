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

        public EmployeeService(UserManager<User> manager, IMapper mapper, IRepository<Employee> repo, ApplicationDbContext context)
        {
            _manager = manager;
            _mapper = mapper;
            _repo = repo;
            _context = context;
        }

        public async Task<IList<Employee>> GetAllAsync()
        {
            return await _repo.Context.Query()
                .OrderByDescending(m => m.Created)
                .Include(m => m.Identity)
                .ToListAsync();
        }

        public async Task<Employee> FindAsync(Guid id)
        {
            return await _repo.Context.GetByIdAsync(id);
        }

        public async Task<bool> isCardExist(Guid id, string cardNo)
        {
            var res = await _repo.Context.Query()
                .Where(m => m.Id != id)
                .Where(m => m.CardNo == cardNo)
                .Where(m => m.Deleted == null)
                .CountAsync();
            return (res > 0) ? true: false;
        }

        public async Task AddAsync(Employee emp)
        {
            try
            {
                _repo.Context.Insert(emp);
                await _repo.SaveAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAsync(EmployeeViewModel model)
        {
            try
            {
                var oldModel = _repo.Context.GetById(model.Id);
                _mapper.Map(model, oldModel);
                oldModel.Updated = DateTime.UtcNow;

                _repo.Context.Update(oldModel);
                await _repo.SaveAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}