using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NotesApp.Tests.E2ETesting
{
    public class EndToEndTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly LoginPage _loginPage;
        private readonly IndexPage _indexPage;

        public EndToEndTests()
        {
            _driver = new ChromeDriver();
            _loginPage = new LoginPage(_driver);
            _indexPage = new IndexPage(_driver);
        }

        //taskkill /f /im chromedriver.exe

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void Create_ValidInput_CreatesNewNote()
        {
            Login();
            _indexPage.Navigate();
            _indexPage.ClickNewNote();
            Assert.Equal("Create - NotesApp", _driver.Title);

            CreatePage createPage = new CreatePage(_driver);
            var title = GenerateRandomString(10);
            var body = GenerateRandomString(40);
            createPage.PopulateTitle(title);
            createPage.PopulateBody(body);
            createPage.ClickCreate();

            Assert.Equal("My Notes - NotesApp", _driver.Title);
            Assert.Contains(title, _driver.PageSource);
            Assert.Contains(body, _driver.PageSource);
        }

        [Fact]
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

        private void Login()
        {
            _loginPage.Navigate();
            _loginPage.PopulateEmail("linagrozdanovska99@gmail.com");
            _loginPage.PopulatePassword("Test123!");
            _loginPage.ClickLogIn();
        }

        private string GenerateRandomString(int length)
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
