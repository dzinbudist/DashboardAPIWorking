using AutoMapper;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Business.DTOs.Logs;
using DashBoard.Business.DTOs.Users;
using DashBoard.Data.Entities;

namespace DashBoard.Web.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModelDto>();
            CreateMap<RegisterModelDto, User>();
            CreateMap<UpdateModelDto, User>();
            CreateMap<DomainModel, DomainModelDto>();
            CreateMap<DomainForCreationDto, DomainModel>();
            CreateMap<DomainForUpdateDto, DomainModel>();
            CreateMap<LogModel, LogModelDto>();
        }
    }
}