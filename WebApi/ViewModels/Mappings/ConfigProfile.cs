using WebApi.Entities;
using AutoMapper;

namespace WebApi.ViewModels
{
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            CreateMap<Configuration, ConfigurationViewModel>();
            CreateMap<ConfigurationViewModel, Configuration>();
        }
    }
}