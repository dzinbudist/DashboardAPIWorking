//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;
//using AutoMapper.Configuration.Annotations;
//using DashBoard.Business.DTOs.Domains;
//using DashBoard.Business.Services;
//using DashBoard.Data.Entities;
//using DashBoard.Web.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Xunit;

//namespace Controllers.Tests
//{
//    public class TestDomainController 
//    {

//        //TestGetDomainModel
//        [Fact]
//        public void TestGetDomainModel_ReturnsOkObjectAndDomainDto() 
//        {
//            //Arrange
//            const int domainId = 123;
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.GetById(domainId, "123"))
//                .Returns(new DomainModelDto(){Id = domainId}); //all other properties are default for simplicity.
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.GetDomainModel(domainId);
//            //Assert
//            var actionResult = Assert.IsType<OkObjectResult>(result);
//            var returnValue = Assert.IsType<DomainModelDto>(actionResult.Value);
//            Assert.Equal(123, returnValue.Id);
//        }
//        [Fact]
//        public void TestGetDomainModel_ReturnsNotFound()
//        {
//            //Arrange
//            var badDomainId = 999;
//            var mockService = new Mock<IDomainService>();
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.GetDomainModel(badDomainId);
//            //Assert
//            Assert.IsType<NotFoundResult>(result);
//        }

//        //TestGetAllNotDeletedDomains

//        [Fact]
//        public void TestGetAllNotDeletedDomains_ReturnsOkObjectAndDomainDtoEnumerable()
//        {
//            //Arrange
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.GetAllNotDeleted("123"))
//                .Returns(DomainModelsForTest());
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.GetAllNotDeletedDomains();
//            //Assert
//            var actionResult = Assert.IsType<OkObjectResult>(result);
//            var returnValue = Assert.IsType<List<DomainModelDto>>(actionResult.Value);
//            Assert.Equal(2, returnValue.Count);
//        }
//        [Fact]
//        public void TestGetAllNotDeletedDomains_ReturnsNotFound()
//        {
//            //Arrange
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.GetAllNotDeleted("123"))
//                .Returns((IEnumerable<DomainModelDto>) null); //needs casting to return null.
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.GetAllNotDeletedDomains();
//            //Assert
//            Assert.IsType<NotFoundResult>(result);
//        }

//        //TestPseudoDeleteDomainModel

//        [Fact]
//        public void TestPseudoDeleteDomainModel_ReturnsOkObject()
//        {
//            //Arrange
//            int domainId = 123;
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.PseudoDelete(domainId, "123"))
//                .Returns(new { message = $"Domain with {domainId} id deleted" }
//            );
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.PseudoDeleteDomainModel(domainId);
//            //Assert
//            var actionResult = Assert.IsType<OkResult>(result);
//        }
//        [Fact]
//        public void TestPseudoDeleteDomainModel_ReturnsNotFound()
//        {
//            //Arrange
//            var badDomainId = 999;
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.PseudoDelete(badDomainId, "123"));
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.PseudoDeleteDomainModel(badDomainId);
//            //Assert
//            Assert.IsType<NotFoundResult>(result);
//        }

//        //TestCreateDomainModel
        
//        [Fact]
//        public void TestCreateDomainModel_ReturnsNewlyCreatedDomain()
//        {
//            //Arrange
//            var newDomain = new DomainForCreationDto()
//            {
//                Service_Name = "TestName",
//                Url = "www.test.com",
//                Notification_Email = "test@test.com"
//            };
//            var userId = "2"; //user id that makes the request
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.Create(newDomain, userId))
//                .Returns(Task.FromResult(new DomainModelDto(){Id = 1}));
//            var controller = new DomainController(mockService.Object);
            
//            //Act
//            var result = controller.CreateDomainModel(newDomain);
//            //Assert
//            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
//            var returnValue = Assert.IsType<DomainModelDto>(createdAtActionResult.Value);
//            Assert.Equal(1, returnValue.Id);
//        }

//        [Fact]
//        public void TestCreateDomainModel_ReturnsBadRequest_GivenInvalidModel()
//        {
//            //Arrange & Act

//            var mockService = new Mock<IDomainService>();
//            var controller = new DomainController(mockService.Object);
//            controller.ModelState.AddModelError("error", "some error");
//            //Act
//            var result = controller.CreateDomainModel(domain:null);
//            //Assert
//            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
//        }
//        //TestEditDomainModel

//        [Fact]
//        public void TestEditDomainModel_ReturnsOkObject() //this might be improved. Didn't found how to deal with controller put methods in documentation.
//        {
//            //Arrange
//            int domainId = 123;
//            var newDomain = new DomainForUpdateDto()
//            {
//                Service_Name = "TestName",
//                Url = "www.test.com",
//                Notification_Email = "test@test.com"
//            };
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.Update(domainId, newDomain, "123"))
//                .Returns(new {message = $"Updated domain with {domainId} id"});
//            var controller = new DomainController(mockService.Object);

//            //Act
//            var result = controller.EditDomainModel(domainId, newDomain);
//            //Assert
//            var actionResult = Assert.IsType<OkResult>(result);
//        }
//        [Fact]
//        public void TestEditDomainModel_ReturnsNotFound() 
//        {
//            //Arrange
//            var nonExistingDomainId = 999;
//            var newDomain = new DomainForUpdateDto()
//            {
//                Service_Name = "TestName",
//                Url = "www.test.com",
//                Notification_Email = "test@test.com"
//            };
//            var mockService = new Mock<IDomainService>();
//            mockService.Setup(service => service.Update(nonExistingDomainId, newDomain, "123"));
//            var controller = new DomainController(mockService.Object);
//            //Act
//            var result = controller.EditDomainModel(nonExistingDomainId, newDomain);
//            //Assert
//            Assert.IsType<NotFoundResult>(result);
//        }

//        [Fact]
//        public void TestEditDomainModel_ReturnsBadRequest_GivenInvalidModel()
//        {
//            //Arrange & Act
//            int randomDomainId = 444;
//            var mockService = new Mock<IDomainService>();
//            var controller = new DomainController(mockService.Object);
//            controller.ModelState.AddModelError("error", "some error");
//            //Act
//            var result = controller.EditDomainModel(randomDomainId, domain: null);
//            //Assert
//            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
//        }
//        private List<DomainModelDto> DomainModelsForTest()
//        {
//            var listOfModels = new List<DomainModelDto>();
//            listOfModels.Add(new DomainModelDto()
//            {
//                Id = 1
//            });
//            listOfModels.Add(new DomainModelDto()
//            {
//                Id = 2
//            });
//            return listOfModels;
//        }
//    }
//}
