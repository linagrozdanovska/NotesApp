using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotesApp.Controllers;
using NotesApp.Data;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NotesApp.Tests
{
    public class UnitTests
    {
        //private readonly Mock<INoteRepository> _mockRepo;

        [Fact]
        public void Home_AuthenticatedUser_RedirectsToIndex()
        {
            //Arrange
            var repo = new Mock<INoteRepository>();

            var store = new Mock<IUserStore<IdentityUser>>();

            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));

            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = (RedirectToActionResult)controller.Home();

            //Assert
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Notes", result.ControllerName);
        }

        [Fact]
        public void Home_UnuthenticatedUser_ReturnsHomeView()
        {
            //Arrange
            var repo = new Mock<INoteRepository>();

            var store = new Mock<IUserStore<IdentityUser>>();

            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Home() as ViewResult;

            //Assert
            Assert.Equal("Home", result.ViewName);
        }
    }
}
