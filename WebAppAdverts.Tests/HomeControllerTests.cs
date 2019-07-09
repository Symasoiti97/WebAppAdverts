using BusinessLogic.Options;
using BusinessLogic.Services;
using DataBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppAdverts.Controllers;
using WebAppAdverts.Models;
using Xunit;

namespace WebAppAdverts.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsAViewResultWithAListOfAdverts()
        {
            //Arrange
            var dbMock = new Mock<IConcreteOperationDb>();
            dbMock.Setup(db => db.GetAdverts()).Returns(GetTestAdverts().AsQueryable());
            var reCaptchaMock = new Mock<IReCaptchaService>();
            var optionsMock = new Mock<IOptions<AppOptions>>();
            optionsMock.Setup(op => op.Value).Returns(GetTestAppOptions());
            var convertMock = new Mock<IConverterService<byte[], IFormFile>>();

            var controller = new HomeController(dbMock.Object, reCaptchaMock.Object,
                             optionsMock.Object, convertMock.Object);

            //Act
            var result = controller.Index("", "");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var indexVM = viewResult.Model as IndexViewModel;
            var model = Assert.IsAssignableFrom<IEnumerable<Advert>>(indexVM.Adverts);
            Assert.Equal(GetTestAdverts().ToList().Count, indexVM.Adverts.Count());
        }

        [Fact]
        public async Task Create_ReturnsViewResultWithCreateVM()
        {
            //Arrange
            var dbMock = new Mock<IConcreteOperationDb>();
            var reCaptchaMock = new Mock<IReCaptchaService>();
            var optionsMock = new Mock<IOptions<AppOptions>>();
            optionsMock.Setup(op => op.Value).Returns(GetTestAppOptions());
            var convertMock = new Mock<IConverterService<byte[], IFormFile>>();

            var controller = new HomeController(dbMock.Object, reCaptchaMock.Object,
                             optionsMock.Object, convertMock.Object);
            controller.ModelState.AddModelError("Content", "Required");
            controller.ModelState.AddModelError("Image", "Required");
            CreateViewModel newCreateVM = new CreateViewModel();
  
            //Act
            var result = await controller.Create(newCreateVM);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(newCreateVM, viewResult?.Model);
        }

        [Fact]
        public void Edit_ReturnsViewResultWithCreateVM()
        {
            //Arrange
            var dbMock = new Mock<IConcreteOperationDb>();
            dbMock.Setup(db => db.GetAdverts()).Returns(GetTestAdverts().AsQueryable());
            var reCaptchaMock = new Mock<IReCaptchaService>();
            var optionsMock = new Mock<IOptions<AppOptions>>();
            optionsMock.Setup(op => op.Value).Returns(GetTestAppOptions());
            var convertMock = new Mock<IConverterService<byte[], IFormFile>>();
            var controller = new HomeController(dbMock.Object, reCaptchaMock.Object,
                             optionsMock.Object, convertMock.Object);
            EditViewModel editVM = new EditViewModel
            {
                AdvertId = new Guid("58667fde-d3f2-4eb3-997f-08d6f101052e"),
                Content = "New Content"
            };

            //Act
            var result = controller.Edit(editVM);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            dbMock.Verify(r => r.UpdateAdvert(It.IsAny<Advert>()));
        }

        [Fact]
        public void Delete_ReturnsViewResultIndex()
        {
            //Arrange
            var dbMock = new Mock<IConcreteOperationDb>();
            dbMock.Setup(db => db.GetAdverts()).Returns(GetTestAdverts().AsQueryable());
            var reCaptchaMock = new Mock<IReCaptchaService>();
            var optionsMock = new Mock<IOptions<AppOptions>>();
            optionsMock.Setup(op => op.Value).Returns(GetTestAppOptions());
            var convertMock = new Mock<IConverterService<byte[], IFormFile>>();

            var controller = new HomeController(dbMock.Object, reCaptchaMock.Object,
                             optionsMock.Object, convertMock.Object);

            //Act
            var result = controller.DeleteConfirmed(new Guid("58666fde-d3f2-4eb3-997f-08d6f101052e"));

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            dbMock.Verify(r => r.DeleteAdvert(It.IsAny<Advert>()));
        }


        private List<Advert> GetTestAdverts()
        {
            var adverts = new List<Advert>()
            {
                 new Advert { Content = "Content1", DateTime = new DateTime(2017,5,6), Rating = 0,
                              Number = 0, Id= new Guid("58666fde-d3f2-4eb3-997f-08d6f101052e"),
                              User  = new User {Id = new Guid("58222fde-d3f2-4eb3-997f-08d6f101052e"),
                              Name = "Admin"} },
                 new Advert { Content = "Content2", DateTime = new DateTime(2017,5,6), Rating = 0,
                              Number = 0, Id= new Guid("58667fde-d3f2-4eb3-997f-08d6f101052e"),
                              User  = new User {Id = new Guid("58222fde-d3f2-4eb3-997f-08d6f101052e"),
                              Name = "Admin"} },
                 new Advert { Content = "Content3", DateTime = new DateTime(2017,5,6), Rating = 0,
                              Number = 0, Id= new Guid("58668fde-d3f2-4eb3-997f-08d6f101052e"),
                              User  = new User {Id = new Guid("58222fde-d3f2-4eb3-997f-08d6f101052e"),
                              Name = "Admin"} }
            };

            return adverts;
        }

        private AppOptions GetTestAppOptions()
        {
            var appOptions = new AppOptions
            {
                IndexOptions = new IndexOptions
                {
                    CountAdvertsByAuthor = 10,
                    CountAdvertsByPage = 10
                },
                ReCaptcha = new ReCaptchaOptions
                {
                    SecretKey = "123",
                    SiteKey = "321"
                }
            };

            return appOptions;
        }

    }
}
