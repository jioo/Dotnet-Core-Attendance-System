using AutoMapper;
using WebApi.Entities;

namespace WebApi.Features.Logs
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