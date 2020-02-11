using AutoMapper;
using DashBoard.Web.Helpers;

namespace Services.Tests.Helpers
{
    public static class AutoMapperConfig
    {
        public static readonly MapperConfiguration Config = new MapperConfiguration(cfg =>
            cfg.AddProfile<AutoMapperProfile>());

    }
}
