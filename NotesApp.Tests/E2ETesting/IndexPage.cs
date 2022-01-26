using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotesApp.Tests.E2ETesting
{
    public class IndexPage
    {
        private readonly IWebDriver _driver;

        public IndexPage(IWebDriver driver) => _driver = driver;
        private const string URI = "https://localhost:44337/Notes/Index";

        private IWebElement SearchElement => _driver.FindElement(By.Id("search"));
        private IWebElement SearchButtonElement => _driver.FindElement(By.Id("search-btn"));
        private IWebElement SearchCancelElement => _driver.FindElement(By.Id("cancel"));
        private IWebElement NewNoteElement => _driver.FindElement(By.Id("new-note"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;

        public void PopulateSearch(string searchString)
        {
            SearchElement.SendKeys(searchString);
            Thread.Sleep(1000);
        }

        public void ClickSearch()
        {
            SearchButtonElement.Click();
            Thread.Sleep(2000);
        }

        public void ClickCancel()
        {
            SearchCancelElement.Click();
            Thread.Sleep(2000);
        }

        public void ClickNewNote()
        {
            NewNoteElement.Click();
            Thread.Sleep(1000);
        }

        public string GetSearchString()
        {
            return SearchElement.GetAttribute("value");
        }

        public void Navigate()
        {
            _driver.Navigate().GoToUrl(URI);
            Thread.Sleep(1000);
        }
    }
}
