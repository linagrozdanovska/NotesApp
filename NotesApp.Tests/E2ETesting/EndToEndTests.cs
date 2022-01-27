using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace NotesApp.Tests.E2ETesting
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class EndToEndTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly LoginPage _loginPage;
        private readonly IndexPage _indexPage;
        private static string _title;
        private static string _body;

        public EndToEndTests()
        {
            _driver = new ChromeDriver();
            _loginPage = new LoginPage(_driver);
            _indexPage = new IndexPage(_driver);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact, Priority(1)]
        public void Index_SearchString_ReturnsMatchingNotesFromUser()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.PopulateSearch("First Note");
            _indexPage.ClickSearch();
            Assert.Equal("My Notes - NotesApp", _indexPage.Title);
            Assert.Contains("First Note", _indexPage.Source);
            Assert.DoesNotContain("Other note", _indexPage.Source);
            Assert.DoesNotContain("Note #3", _indexPage.Source);
        }

        [Fact, Priority(2)]
        public void Index_EmptySearchString_ReturnsAllNotesFromUser()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.PopulateSearch("");
            _indexPage.ClickSearch();
            Assert.Equal("My Notes - NotesApp", _indexPage.Title);
            Assert.Contains("First Note", _indexPage.Source);
            Assert.Contains("Other note", _indexPage.Source);
            Assert.Contains("Note #3", _indexPage.Source);
        }

        [Fact, Priority(3)]
        public void Index_CancelSearch_ClearsInputAndReturnsAllNotes()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.PopulateSearch("bla bla bla bla bla...");
            _indexPage.ClickCancel();
            var searchString = _indexPage.GetSearchString();
            Assert.Equal("My Notes - NotesApp", _indexPage.Title);
            Assert.Equal("", searchString);
            Assert.Contains("First Note", _indexPage.Source);
            Assert.Contains("Other note", _indexPage.Source);
            Assert.Contains("Note #3", _indexPage.Source);
        }

        [Fact, Priority(4)]
        public void Create_ValidInput_CreatesNewNote()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.ClickNewNote();
            Assert.Equal("Create - NotesApp", _driver.Title);

            CreatePage createPage = new CreatePage(_driver);
            string title = GenerateRandomString(10);
            _title = title;
            string body = GenerateRandomString(30);
            _body = body;
            createPage.PopulateTitle(title);
            createPage.PopulateBody(body);
            createPage.ClickCreate();

            Assert.Equal("My Notes - NotesApp", _driver.Title);
            Assert.Contains(title, _driver.PageSource);
            Assert.Contains(body, _driver.PageSource);
        }

        [Fact, Priority(5)]
        public void Create_EmptyInput_ShowsErrors()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.ClickNewNote();
            Assert.Equal("Create - NotesApp", _driver.Title);

            CreatePage createPage = new CreatePage(_driver);
            var title = "";
            var body = "";
            createPage.PopulateTitle(title);
            createPage.PopulateBody(body);
            createPage.ClickCreate();

            Assert.Equal("Create - NotesApp", _driver.Title);
            Assert.Equal("The Title field is required.", createPage.TitleErrorMessage);
            Assert.Equal("The Body field is required.", createPage.BodyErrorMessage);
        }

        [Fact, Priority(6)]
        public void Edit_ValidInput_SavesNoteAndReturnsToIndex()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.ClickNoteLink(_title);
            Assert.Equal("Details - NotesApp", _driver.Title);

            DetailsPage detailsPage = new DetailsPage(_driver);
            detailsPage.ClickEdit();
            Assert.Equal("Edit - NotesApp", _driver.Title);

            EditPage editPage = new EditPage(_driver);
            editPage.ClearTitle();
            editPage.ClearBody();
            string title = GenerateRandomString(10);
            _title = title;
            string body = GenerateRandomString(30);
            _body = body;
            editPage.PopulateTitle(title);
            editPage.PopulateBody(body);
            editPage.ClickSave();

            Assert.Equal("My Notes - NotesApp", _driver.Title);
            Assert.Contains(title, _driver.PageSource);
            Assert.Contains(body, _driver.PageSource);
        }

        [Fact, Priority(7)]
        public void Edit_EmptyInput_ShowsErrors()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.ClickNoteLink(_title);
            Assert.Equal("Details - NotesApp", _driver.Title);

            DetailsPage detailsPage = new DetailsPage(_driver);
            detailsPage.ClickEdit();
            Assert.Equal("Edit - NotesApp", _driver.Title);

            EditPage editPage = new EditPage(_driver);
            editPage.ClearTitle();
            editPage.ClearBody();
            editPage.ClickSave();

            Assert.Equal("Edit - NotesApp", _driver.Title);
            Assert.Equal("The Title field is required.", editPage.TitleErrorMessage);
            Assert.Equal("The Body field is required.", editPage.BodyErrorMessage);
        }

        private void Login()
        {
            _loginPage.Navigate();
            _loginPage.PopulateEmail("linagrozdanovska99@gmail.com");
            _loginPage.PopulatePassword("Test123!");
            _loginPage.ClickLogIn();
        }

        private static string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
