using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface IConfigService
    {
        Task<Configuration> FirstOrDefaultAsync();
        Task UpdateAsync(ConfigurationViewModel model);
    }

}