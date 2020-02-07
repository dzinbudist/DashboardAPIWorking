using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DashBoard.Business.DTOs.Logs;
using DashBoard.Business.Services;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;
using DashBoard.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Services.Tests
{
    public class TestLogsService
    {
        [Fact]
        public void TestGetAll()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestGetAll")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            using (var context = new DataContext(options))
            {
                context.Logs.AddRange(SeedEntitiesLogModels());
                context.SaveChanges();
            }

            //Act
            using (var context = new DataContext(options))
            {
                var service = new LogsService(context, mapper);
                var result = service.GetAllLogs();
                //Assert
                Assert.Equal(4, context.Logs.Count()); 
                Assert.IsType<List<LogModelDto>>(result);
            }
        }
        [Fact]
        public void TestGetLogsByDomainId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: "TestGetLogsByDomainId")
                .Options;

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<AutoMapperProfile>());

            var mapper = config.CreateMapper(); // according to some people this is better than mocking automapper.
            using (var context = new DataContext(options))
            {
                context.Logs.AddRange(SeedEntitiesLogModels());
                context.SaveChanges();
            }

            //Act
            using (var context = new DataContext(options))
            {
                var service = new LogsService(context, mapper);
                var result = service.GetLogsByDomainId(52);
                //Assert
                Assert.Equal(2, result.Count());
                Assert.Equal(52, result.First().Domain_Id);
                Assert.IsType<List<LogModelDto>>(result);
            }
        }
        private IEnumerable<LogModel> SeedEntitiesLogModels()
        {
            var list = new List<LogModel>();
            list.Add(new LogModel { Id = 1, Domain_Id = 51, Error_Text = null, Log_Date = DateTime.Now});
            list.Add(new LogModel { Id = 2, Domain_Id = 51, Error_Text = null, Log_Date = DateTime.Now });
            list.Add(new LogModel { Id = 3, Domain_Id = 52, Error_Text = null, Log_Date = DateTime.Now });
            list.Add(new LogModel { Id = 4, Domain_Id = 52, Error_Text = null, Log_Date = DateTime.Now });
            return list;
        }
    }
}
