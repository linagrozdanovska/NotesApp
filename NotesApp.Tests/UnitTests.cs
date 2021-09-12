using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotesApp.Controllers;
using NotesApp.Data;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [InlineData("")]
        public void Index_EmptySearchString_ReturnsAllNotesFromUser(string searchstring)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.GetAll(user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Index(searchstring);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var notes = Assert.IsType<List<Note>>(viewResult.Model);
            Assert.Equal(3, notes.Count);
        }

        [Theory]
        [InlineData(":)")]
        public void Index_SearchString_ReturnsNotesContainingSearchStringFromUser(string searchstring)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.GetAll(user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Index(searchstring);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var notes = Assert.IsType<List<Note>>(viewResult.Model);
            Assert.Equal(2, notes.Count);
            Assert.All(notes, item => Assert.True(item.Title.Contains(searchstring) || item.Body.Contains(searchstring)));
        }

        private List<Note> GetTestNotes(string userId)
        {
            List<Note> output = new List<Note>
            {
                new Note
                {
                    Id = 0,
                    UserId = "test",
                    Date = DateTime.Now,
                    Title = "Test Note 1",
                    Body = "Body of note 1."
                },
                new Note
                {
                    Id = 1,
                    UserId = "diff",
                    Date = DateTime.Now,
                    Title = "Note 1",
                    Body = "Lorem ipsum dolor sit amet."
                },
                new Note
                {
                    Id = 2,
                    UserId = "test",
                    Date = DateTime.Now,
                    Title = "Test Note 2",
                    Body = "Hello :)."
                },
                new Note
                {
                    Id = 3,
                    UserId = "test",
                    Date = DateTime.Now,
                    Title = "Test Note 3",
                    Body = "This is note 3 :)."
                }
            };
            return output.Where(z => z.UserId.Equals(userId)).ToList();
        }
    }
}
