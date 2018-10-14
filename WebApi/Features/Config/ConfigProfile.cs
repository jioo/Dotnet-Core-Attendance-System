using WebApi.Entities;
using AutoMapper;

namespace WebApi.Features.Config
{
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            CreateMap<Entities.Config, ConfigViewModel>();
            CreateMap<ConfigViewModel, Entities.Config>();
        }
    }
}