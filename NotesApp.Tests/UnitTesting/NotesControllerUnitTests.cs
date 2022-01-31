using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NotesApp.Controllers;
using NotesApp.Data;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace NotesApp.Tests
{
    public class NotesControllerUnitTests
    {
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

        [Theory]
        [InlineData(null)]
        public void Details_NullId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var controller = new NotesController(repo.Object, manager.Object);

            //Act
            var result = controller.Details(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(5)]
        public void Details_NonexistentId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Details(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public void Details_ExistentId_ReturnsDetailsView(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Details(id) as ViewResult;

            //Assert
            Assert.Equal("Details", result.ViewName);
            var note = Assert.IsType<Note>(result.Model);
            Assert.Equal(id, note.Id);
        }

        [Fact]
        public void Create_Get_ReturnsCreateView()
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var controller = new NotesController(repo.Object, manager.Object);

            //Act
            var result = controller.Create() as ViewResult;

            //Assert
            Assert.Equal("Create", result.ViewName);
        }

        [Fact]
        public void Create_Post_InvalidData_ReturnsCreateView()
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            Note n = null;
            repo.Setup(r => r.Insert(It.IsAny<Note>()))
                .Callback<Note>(x => n = x);
            Note note = new Note
            {
                Title = "Title"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            controller.ModelState.AddModelError("Body", "The Body field is required.");

            //Act
            var result = controller.Create(note) as ViewResult;

            //Assert
            repo.Verify(x => x.Insert(It.IsAny<Note>()), Times.Never);
            Assert.Equal("Create", result.ViewName);
            Assert.IsType<Note>(result.Model);
        }

        [Fact]
        public void Create_Post_ValidData_RedirectsToIndexAction()
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            Note n = null;
            repo.Setup(r => r.Insert(It.IsAny<Note>()))
                .Callback<Note>(x => n = x);
            Note note = new Note
            {
                Title = "Title",
                Body = "Body"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            RedirectToActionResult result = (RedirectToActionResult)controller.Create(note);

            //Assert
            repo.Verify(x => x.Insert(It.IsAny<Note>()), Times.Once);
            Assert.Equal(n.Title, note.Title);
            Assert.Equal(n.Body, note.Body);
            Assert.Equal(n.UserId, user.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.Equal("Index", result.ActionName);
        }

        [Theory]
        [InlineData(null)]
        public void Edit_Get_NullId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var controller = new NotesController(repo.Object, manager.Object);

            //Act
            var result = controller.Edit(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(5)]
        public void Edit_Get_NonexistentId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Edit(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public void Edit_Get_ExistentId_ReturnsEditView(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Edit(id) as ViewResult;

            //Assert
            Assert.Equal("Edit", result.ViewName);
            var note = Assert.IsType<Note>(result.Model);
            Assert.Equal(id, note.Id);
        }

        [Theory]
        [InlineData(2)]
        public void Edit_Post_DifferentIdValues_ReturnsNotFound(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            Note note = new Note
            {
                Id = 0,
                UserId = "test",
                Date = DateTime.Now,
                Title = "Title",
                Body = "Body"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Edit(id, note);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public void Edit_Post_InvalidModelState_ReturnsEditView(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            Note n = null;
            repo.Setup(r => r.Update(It.IsAny<Note>()))
                .Callback<Note>(x => n = x);
            Note note = new Note
            {
                Id = 0,
                UserId = "test",
                Date = DateTime.Now,
                Title = "Title"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            controller.ModelState.AddModelError("Body", "The Body field is required.");

            //Act
            var result = controller.Edit(id, note) as ViewResult;

            //Assert
            repo.Verify(x => x.Update(It.IsAny<Note>()), Times.Never);
            Assert.Equal("Edit", result.ViewName);
            Assert.IsType<Note>(result.Model);
            Assert.Equal(id, note.Id);
        }

        [Theory]
        [InlineData(0)]
        public void Edit_Post_ValidModelStateNoException_RedirectsToIndexAction(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            Note n = null;
            repo.Setup(r => r.Update(It.IsAny<Note>()))
                .Callback<Note>(x => n = x);
            Note note = new Note
            {
                Id = 0,
                UserId = "test",
                Date = DateTime.Now,
                Title = "Title",
                Body = "Body"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            RedirectToActionResult result = (RedirectToActionResult)controller.Edit(id, note);

            //Assert
            repo.Verify(x => x.Update(It.IsAny<Note>()), Times.Once);
            Assert.Equal(n.Id, note.Id);
            Assert.Equal(n.Title, note.Title);
            Assert.Equal(n.Body, note.Body);
            Assert.Equal(n.UserId, user.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.Equal("Index", result.ActionName);
        }

        [Theory]
        [InlineData(10)]
        public void Edit_Post_ValidModelStateExceptionNonexistentNote_ReturnsNotFound(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Update(It.IsAny<Note>())).Throws<DbUpdateConcurrencyException>();
            repo.Setup(r => r.GetAll(user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)));
            Note note = new Note
            {
                Id = 10,
                UserId = "test",
                Date = DateTime.Now,
                Title = "Title",
                Body = "Body"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Edit(id, note);

            //Assert
            repo.Verify(x => x.Update(It.IsAny<Note>()), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public void Edit_Post_ValidModelStateExceptionExistentNote_ThrowsException(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Update(It.IsAny<Note>())).Throws<DbUpdateConcurrencyException>();
            repo.Setup(r => r.GetAll(user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)));
            Note note = new Note
            {
                Id = 0,
                UserId = "test",
                Date = DateTime.Now,
                Title = "Title",
                Body = "Body"
            };
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            Action result = () => controller.Edit(id, note);

            //Assert
            DbUpdateConcurrencyException exception = Assert.Throws<DbUpdateConcurrencyException>(result);
        }

        [Theory]
        [InlineData(null)]
        public void Delete_NullId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var controller = new NotesController(repo.Object, manager.Object);

            //Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(5)]
        public void Delete_NonexistentId_ReturnsNotFound(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Delete(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public void Delete_ExistentId_ReturnsDeleteView(int? id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            var result = controller.Delete(id) as ViewResult;

            //Assert
            Assert.Equal("Delete", result.ViewName);
            var note = Assert.IsType<Note>(result.Model);
            Assert.Equal(id, note.Id);
        }

        [Theory]
        [InlineData(0)]
        public void DeleteConfirmed_ExistentNote_RedirectsToIndexAction(int id)
        {
            //Arrange
            var repo = new Mock<INoteRepository>();
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Name, "test@mail.com")
            }, "TestAuthentication"));
            repo.Setup(r => r.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Returns(GetTestNotes(user.FindFirstValue(ClaimTypes.NameIdentifier)).SingleOrDefault(z => z.Id.Equals(id)));
            Note n = null;
            repo.Setup(r => r.Delete(It.IsAny<Note>()))
                .Callback<Note>(x => n = x);
            var controller = new NotesController(repo.Object, manager.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            //Act
            RedirectToActionResult result = (RedirectToActionResult)controller.DeleteConfirmed(id);

            //Assert
            repo.Verify(x => x.Get(id, user.FindFirstValue(ClaimTypes.NameIdentifier)), Times.Once);
            repo.Verify(x => x.Delete(It.IsAny<Note>()), Times.Once);
            Assert.Equal(0, n.Id);
            Assert.Equal("test", n.UserId);
            Assert.Equal("Index", result.ActionName);
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
