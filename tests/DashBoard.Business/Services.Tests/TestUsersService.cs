using System;
using System.Collections.Generic;
using System.Linq;
using DashBoard.Business.DTOs.Users;
using DashBoard.Business.Services;
using DashBoard.Data.Data;
using DashBoard.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Tests.Helpers;
using Xunit;

namespace Services.Tests
{
    public class TestUsersService
    {

        [Fact]
        public void TestCreateAndAuthenticate() 
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.


            var newUser = new RegisterModelDto()
            {
                Username = "anon1", FirstName = "fakename", LastName = "fakelastname", Password = "somepassword"
            };
            
            //Act
            using (var context = new DataContext(dbOptions))
            {
                var userId = "2";
                var service = new  UserService(context, mapper);
                var createResult = service.Create(newUser, newUser.Password, userId); //authenticate needs creation first. This ain't ideal for testing.
                var authenticateResult = service.Authenticate(newUser.Username, newUser.Password);
                var badAuthenticateResult = service.Authenticate(newUser.Username, "badpassword");

                //Assert
                Assert.IsType<User>(createResult);
                Assert.Equal(1, context.Users.Count());
                Assert.IsType<User>(authenticateResult);
                Assert.Null(badAuthenticateResult);
            }
        }

        [Fact]
        public void TestGetAllAndGetById()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            var newUser1 = new RegisterModelDto()
                {Username = "anon1", FirstName = "fakename1", LastName = "fakelastname1", Password = "somepassword1"};
            var newUser2 = new RegisterModelDto()
                {Username = "anon2", FirstName = "fakename2", LastName = "lastname2", Password = "somepassword2"};

            //Act
            using (var context = new DataContext(dbOptions))
            {
                var service = new UserService(context, mapper);
                var userId = "2";
                service.Create(newUser1, newUser1.Password, userId); //this might need to be mocked in the future.
                service.Create(newUser2, newUser2.Password, userId);
                var getByIdResult = service.GetById(1);
                var getAllResult = service.GetAll();

                //Assert
                Assert.IsType<UserModelDto>(getByIdResult);
                Assert.Equal("anon1", getByIdResult.Username);
                Assert.IsType<List<UserModelDto>>(getAllResult);
                Assert.Equal(2, getAllResult.Count());
            }
        }
        [Fact]
        public void TestDeleteAndUpdate() //low quality test. Might need factoring out in the future the whole UserSerivce.Create() method and this test.
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            var newUser1 = new RegisterModelDto()
                { Username = "anon1", FirstName = "fakename1", LastName = "fakelastname1", Password = "somepassword1" };
            var newUser2 = new RegisterModelDto()
                { Username = "anon2", FirstName = "fakename2", LastName = "lastname2", Password = "somepassword2" };

            var updateUser = new UpdateModelDto()
                {Username = "anon2", FirstName = "newName", LastName = "lastname2", Password = "somepassword2"};
            //Act
            using (var context = new DataContext(dbOptions))
            {
                var userId = "2";
                var service = new UserService(context, mapper);
                service.Create(newUser1, newUser1.Password, userId); //this might need to be mocked in the future.
                service.Create(newUser2, newUser2.Password, userId);
                service.Delete(1);
                service.Update(2, updateUser, userId);

                //Assert
                Assert.Equal("anon2", context.Users.Find(2).Username);
                Assert.Equal("newName", context.Users.Find(2).FirstName);
                Assert.Equal(1, context.Users.Count());
            }
        }
    }
}
