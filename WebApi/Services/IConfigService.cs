using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.ViewModels;

namespace WebApi.Services
{
    public interface IConfigService
    {
        /// <summary>
        /// Get the first record in Configuration
        /// </summary>
        Task<Configuration> FirstOrDefaultAsync();

        /// <summary>
        /// Update existing Configuration
        /// </summary>
        /// <param name="viewModel">view model</param>
        /// <returns>
        /// Returns <see cref="ConfigurationViewModel"/>
        /// </returns>
        Task UpdateAsync(ConfigurationViewModel viewModel);
    }

}