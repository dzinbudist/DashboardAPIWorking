using Moq;
using System.Collections.Generic;
using DashBoard.Business.CustomExceptions;
using DashBoard.Business.DTOs.Users;
using DashBoard.Business.Services;
using DashBoard.Data.Entities;
using DashBoard.Web.Controllers;
using DashBoard.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace Controllers.Tests
{
    public class TestUsersController
    {
        [Fact]
        void TestAuthenticate_ReturnsOkResultObject()
        {
            var user = new AuthenticateModelDto()
            {
                Username = "testUsername",
                Password = "testPassword"
            };
            var newUser = new User();

            //Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(newUser);//service returns user. Left empty, since we're only unit testing action results.
            var mockTokenService = new Mock<ITokenService>();
            mockTokenService.Setup(service => service.GenerateToken(It.IsAny<User>(), new AppSettings().Secret))
                .Returns("fakeJwtToken1312313123");
            var mockAppSettings = new Mock<IOptions<AppSettings>>();
            mockAppSettings.Setup(settings => settings.Value).Returns(new AppSettings());

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);

            //Act
            var result = controller.Authenticate(user);
            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        void TestAuthenticate_ReturnsBadRequestObjectResult()
        {
            //Arrange
            var mockUserService = new Mock<IUserService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var user = new AuthenticateModelDto()
            {
                Username = "testUsername",
                Password = "testPassword"
            };

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Authenticate(user);
            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        void TestGetAll_ReturnsOkResultObjectWithEnumerable()
        {
            //Arrange
            var testUsers = new List<UserModelDto>() {new UserModelDto(){FirstName = "test1"}, new UserModelDto(){FirstName = "test2"}};
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetAll())
                .Returns(testUsers);
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();



            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.GetAll();
            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<UserModelDto>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        void TestGetById_ReturnsOkResultObject()
        {
            //Arrange
            var userId = 12;
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetById(userId))
                .Returns(new UserModelDto(){Id = userId});
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.GetById(userId);
            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserModelDto>(actionResult.Value);
            Assert.Equal(12, returnValue.Id);
        }
        [Fact]
        void TestUpdate_ReturnsOkResult()
        {
            //Arrange
            var updatedUser = new UpdateModelDto()
            {
                FirstName = "newName",
                LastName = "newLastName"
            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Update(1, updatedUser));
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Update(1, updatedUser);
            //Assert
            var actionResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        void TestUpdate_ReturnsBadRequestObjectResult()
        {
            //Arrange
            var wrongUpdatedUser = new UpdateModelDto()
            {
                FirstName = "newName",
                LastName = "newLastName"
            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Update(1, wrongUpdatedUser))
                .Throws<AppException>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Update(1, wrongUpdatedUser);
            //Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        void TestDelete_ReturnsOkResult()
        {
            //Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Delete(1));
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Delete(1);
            //Assert
            var actionResult = Assert.IsType<OkResult>(result);
        }
        [Fact]
        void TestRegister_ReturnsOkResult()
        {
            //Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Create(It.IsAny<RegisterModelDto>(), "testPassword"))
                .Returns(new User());
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Register(new RegisterModelDto());
            //Assert
            var actionResult = Assert.IsType<OkResult>(result);
        }
        [Fact]
        void TestRegister_ReturnsBadRequest()
        {
            //Arrange
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.Create(It.IsAny<RegisterModelDto>(), It.IsAny<string>()))
                .Throws(new AppException());
            var mockTokenService = new Mock<ITokenService>();
            var mockAppSettings = new Mock<IOptions<AppSettings>>();

            var controller = new UsersController(mockUserService.Object, mockAppSettings.Object, mockTokenService.Object);
            //Act
            var result = controller.Register(new RegisterModelDto());
            //Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
