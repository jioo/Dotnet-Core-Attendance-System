using WebApi.Entities;
using AutoMapper;

namespace WebApi.ViewModels
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Log, LogViewModel>();
            CreateMap<LogViewModel, Log>();

            CreateMap<Log, LogEditViewModel>();
            CreateMap<LogEditViewModel, Log>();
        }
    }
}