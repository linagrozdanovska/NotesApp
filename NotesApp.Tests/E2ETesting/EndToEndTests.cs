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

        private void Login()
        {
            _loginPage.Navigate();
            _loginPage.PopulateEmail("linagrozdanovska99@gmail.com");
            _loginPage.PopulatePassword("Test123!");
            _loginPage.ClickLogIn();
        }
    }
}
