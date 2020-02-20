using System;
using System.Collections.Generic;
using System.Linq;
using DashBoard.Business;
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
        public void TestCreate()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // 

            var newAdmin = new RegisterModelDto()
            {
                Username = "user1",
                FirstName = "fakename",
                LastName = "fakelastname",
                Password = "randomPassw0rd!",
                ConfirmPassword = "randomPassw0rd!",
                CreatedByAdmin = false
            };
            var newUser = new RegisterModelDto()
            {
                Username = "admin1",
                FirstName = "fakename",
                LastName = "fakelastname",
                Password = "randomPassw0rd!",
                ConfirmPassword = "randomPassw0rd!",
                CreatedByAdmin = true
            };

            //Act
            using (var context = new DataContext(dbOptions))
            {
                var service = new UserService(context, mapper);
                var createAdminResult = service.Create(newAdmin, newAdmin.Password, "1");
                var createUserResult = service.Create(newUser, newUser.Password, "1");

                //Assert
                Assert.IsType<User>(createUserResult);
                Assert.IsType<User>(createAdminResult);
                Assert.Equal(2, context.Users.Count());
                Assert.Equal(createAdminResult.Role, Role.Admin);
                Assert.Equal(createUserResult.Role, Role.User);

            }
        }
        [Fact]
        public void TestAuthenticate() 
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            //Act
            using (var context = new DataContext(dbOptions))
            {
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
                var service = new UserService(context, mapper);

                var authenticateUserResult = service.Authenticate("user1", "Passw0rd!?"); //login values from seed models internal class bellow.
                var authenticateAdminResult = service.Authenticate("admin1", "Passw0rd!?");
                var badAuthenticateResult = service.Authenticate("badUsername", "BadPassword");
                
                //Assert
                Assert.IsType<User>(authenticateUserResult);
                Assert.IsType<User>(authenticateAdminResult);
                Assert.Null(badAuthenticateResult);
                Assert.Equal(Role.Admin, authenticateAdminResult.Role);
                Assert.Equal(Role.User, authenticateUserResult.Role);
            }
        }

        [Fact]
        public void TestGetAll()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            //Act
            using (var context = new DataContext(dbOptions))
            {
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
                var service = new UserService(context, mapper);
                var getAllResult = service.GetAll("1"); //returns users with id:1 team members. 
                //Assert
                Assert.IsType<List<UserModelDto>>(getAllResult);
                Assert.Equal(2, getAllResult.Count());
            }
        }
        [Fact]
        public void TestGetById()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            //Act
            using (var context = new DataContext(dbOptions))
            {
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
                var service = new UserService(context, mapper);
                var getAllResult = service.GetById(2);
                //Assert
                Assert.IsType<UserModelDto>(getAllResult);
                Assert.Equal(2, getAllResult.Id);
            }
        }
        [Fact]
        public void TestUpdate()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<DataContext>() //instead of mocking we use inMemoryDatabase.
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) //random db name.
                .Options;
            var mapperConfig = AutoMapperConfig.Config;
            var mapper = mapperConfig.CreateMapper(); // according to some people this is better than mocking automapper.

            var updateUser = new UpdateModelDto()
                { Username = "user1", FirstName = "newName", LastName = "lastname2", Password = "Passw0rd!?", ConfirmPassword = "Passw0rd!?"}; //again, password from seed models in internal class bellow.
            //Act
            using (var context = new DataContext(dbOptions))
            {
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
                var service = new UserService(context, mapper);
                service.Update(2, updateUser, "2");

                //Assert
                Assert.Equal("user1", context.Users.Find(2).Username);
                Assert.Equal("newName", context.Users.Find(2).FirstName);
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

            //Act
            using (var context = new DataContext(dbOptions))
            {
                context.Users.AddRange(SeedFakeData.SeedEntitiesUsersModels());
                context.SaveChanges();
                var service = new UserService(context, mapper);
                service.Delete(2, "1"); //check this 

                //Assert
                Assert.Equal(2, context.Users.Count());
            }
        }
        internal static class SeedFakeData
        {
            private static readonly Guid FirstTeamKey = Guid.NewGuid();
            private static readonly Guid SecondTeamKey = Guid.NewGuid();
            private static readonly string UserPassword = "Passw0rd!?"; //for simplicity, let's give all users same password.

            //creates fake users for database
            internal static IEnumerable<User> SeedEntitiesUsersModels()
            {

                //generate hash and salt for UserPassword
                byte[] generatedHash, generatedSalt;
                CreatePasswordHash(UserPassword, out generatedHash, out generatedSalt);

                var list = new List<User>
                {
                    //first team: 1 admin, 1 user; second team: 1 admin
                    new User() { Id = 1, Username = "admin1", Team_Key = FirstTeamKey, Role = "Admin", PasswordHash = generatedHash, PasswordSalt = generatedSalt},
                    new User() { Id = 2, Username = "user1", Team_Key = FirstTeamKey, Role = "User", PasswordHash = generatedHash, PasswordSalt = generatedSalt},
                    new User() { Id = 3, Username = "admin2",Team_Key = SecondTeamKey, Role = "Admin", PasswordHash = generatedHash, PasswordSalt = generatedSalt}
                };
                return list;
            }

            //These methods are for password hashing/salting. They are copied from User Service. It's not DRY, but didn't want to make those methods public.
            internal static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
            {
                if (password == null) throw new ArgumentNullException("password");
                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

                using (var hmac = new System.Security.Cryptography.HMACSHA512())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                }
            }

            internal static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
            {
                if (password == null) throw new ArgumentNullException("password");
                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
                if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
                if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

                using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
                {
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i]) return false;
                    }
                }

                return true;
            }

        }
    }
}
