using WebApi.Entities;
using AutoMapper;

namespace WebApi.Features.Configurations
{
    public class ConfigurationProfile : Profile
    {
        public ConfigurationProfile()
        {
            CreateMap<Configuration, ConfigurationViewModel>();
            CreateMap<ConfigurationViewModel, Configuration>();
        }
    }
}