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

namespace WebApi.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Configuration> _repo;
        public ConfigService(IMapper mapper, IRepository<Configuration> repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<Configuration> FirstOrDefaultAsync()
        {
            try
            {
                return await _repo.Context.Query().FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAsync(ConfigurationViewModel model)
        {
            try
            {
                var oldModel = _repo.Context.GetById(model.Id);
                _mapper.Map(model, oldModel);

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