using AutoMapper;
using WebApi.Business.DTOs.Domains;
using WebApi.Business.DTOs.Users;
using WebApi.Data.Entities;

namespace WebApi.Helpers
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
        }
    }
}