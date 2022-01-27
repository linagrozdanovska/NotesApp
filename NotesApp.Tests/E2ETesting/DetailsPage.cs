using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotesApp.Tests.E2ETesting
{
    public class DetailsPage
    {
        private readonly IWebDriver _driver;

        public DetailsPage(IWebDriver driver) => _driver = driver;

        private IWebElement EditElement => _driver.FindElement(By.LinkText("Edit"));
        private IWebElement DeleteElement => _driver.FindElement(By.LinkText("Delete"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;

        public void ClickEdit()
        {
            EditElement.Click();
            Thread.Sleep(2000);
        }
        public void ClickDelete()
        {
            DeleteElement.Click();
            Thread.Sleep(2000);
        }
    }
}
