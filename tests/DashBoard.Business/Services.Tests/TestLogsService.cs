using System;
using System.Collections.Generic;
using System.Linq;
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
    //I recommend checking helper class at the bottom and get familiar with the seed data we use to populate databases in tests. When you know what data we have, these tests are self explanatory.

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
                context.Logs.AddRange(SeedFakeData.SeedEntitiesLogModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
            }

            //Act
            using (var context = new DataContext(options))
            {
                var service = new LogsService(context, mapper);
                var result = service.GetAllLogs("1"); 
                //Assert
                Assert.Equal(1, result.Count()); 
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
                context.Logs.AddRange(SeedFakeData.SeedEntitiesLogModels());
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
            }

            //Act
            using (var context = new DataContext(options))
            {
                var service = new LogsService(context, mapper);
                var result = service.GetLogsByDomainId(52, "3");
                //Assert
                Assert.Equal(2, result.Count()); //domain id:52 and second team_key
                Assert.Equal(52, result.First().Domain_Id);
                Assert.IsType<List<LogModelDto>>(result);
            }
        }

        internal static class SeedFakeData
        {
            private static readonly Guid FirstTeamKey = Guid.NewGuid();
            private static readonly Guid SecondTeamKey = Guid.NewGuid();

            //creates fake users for database
            internal static IEnumerable<User> SeedEntitiesUsersModels()
            {
                var list = new List<User>
                {
                    new User() { Id = 1, Team_Key = FirstTeamKey },
                    new User() { Id = 2, Team_Key = FirstTeamKey },
                    new User() { Id = 3, Team_Key = SecondTeamKey }
                };
                return list;
            }
            //creates fake logs for database
            internal static IEnumerable<LogModel> SeedEntitiesLogModels()
            {
                var list = new List<LogModel>();
                list.Add(new LogModel { Id = 1, Domain_Id = 51, Error_Text = null, Log_Date = DateTime.Now, Team_Key = FirstTeamKey});
                list.Add(new LogModel { Id = 2, Domain_Id = 51, Error_Text = null, Log_Date = DateTime.Now, Team_Key = SecondTeamKey});
                list.Add(new LogModel { Id = 3, Domain_Id = 52, Error_Text = null, Log_Date = DateTime.Now, Team_Key = SecondTeamKey});
                list.Add(new LogModel { Id = 4, Domain_Id = 52, Error_Text = null, Log_Date = DateTime.Now, Team_Key = SecondTeamKey});
                return list;
            }

        }
    }
}
