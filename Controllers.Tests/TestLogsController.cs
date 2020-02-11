using System;
using System.Collections.Generic;
using System.Text;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Business.DTOs.Logs;
using DashBoard.Business.Services;
using DashBoard.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Controllers.Tests
{
    public class TestLogsController
    {
        [Fact]
        public void TestGetAllLogs_ReturnsNotFoundResult()
        {
            //Arrange
            var mockLogsService = new Mock<ILogsService>();
            mockLogsService.Setup(service => service.GetAllLogs())
                .Returns((IEnumerable<LogModelDto>)null);
            var controller = new LogsController(mockLogsService.Object);

            //Act
            var result = controller.GetAllLogs();

            //Assert
            var actionResult = Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void TestGetAllLogs_ReturnsOkObjectResult()
        {

            //Arrange
            var logs = new List<LogModelDto>() {new LogModelDto(), new LogModelDto()};
            var mockLogsService = new Mock<ILogsService>();
            mockLogsService.Setup(service => service.GetAllLogs())
                .Returns(logs);
            var controller = new LogsController(mockLogsService.Object);

            //Act
            var result = controller.GetAllLogs();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<LogModelDto>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public void TestGetLogsByDomain_ReturnsOkObjectResult()
        {

            //Arrange
            var logsFor123Id = new List<LogModelDto>(){ new LogModelDto(){Id = 1, Domain_Id = 123, Error_Text = "error"}, new LogModelDto(){Id = 2, Domain_Id = 123}};
            var mockLogsService = new Mock<ILogsService>();
            mockLogsService.Setup(service => service.GetLogsByDomainId(123))
                .Returns(logsFor123Id);
            var controller = new LogsController(mockLogsService.Object);

            //Act
            var result = controller.GetLogsByDomain(123);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<LogModelDto>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public void TestGetLogsByDomain_ReturnsNotFoundResult() //this might be bad and pointless.
        {
            //Arrange
            var nonExistingDomainId = 999;
            var mockLogsService = new Mock<ILogsService>();
            mockLogsService.Setup(service => service.GetLogsByDomainId(nonExistingDomainId))
                .Returns((IEnumerable<LogModelDto>)null);
            var controller = new LogsController(mockLogsService.Object);

            //Act
            var result = controller.GetLogsByDomain(nonExistingDomainId);

            //Assert
            var actionResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
