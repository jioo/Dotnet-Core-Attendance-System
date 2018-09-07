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

        /// <summary>
        /// <see cref="IConfigService.FirstOrDefaultAsync"/>
        /// </summary>
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

        /// <summary>
        /// <see cref="IConfigService.UpdateAsync"/>
        /// </summary>
        public async Task UpdateAsync(ConfigurationViewModel viewModel)
        {
            try
            {
                var model = _repo.Context.GetById(viewModel.Id);
                _mapper.Map(viewModel, model);

                _repo.Context.Update(model);
                await _repo.SaveAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}