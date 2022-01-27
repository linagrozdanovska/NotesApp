using OpenQA.Selenium;
using System.Threading;

namespace NotesApp.Tests.E2ETesting
{
    public class DeletePage
    {
        private readonly IWebDriver _driver;

        public DeletePage(IWebDriver driver) => _driver = driver;

        private IWebElement NoElement => _driver.FindElement(By.LinkText("No"));
        private IWebElement YesElement => _driver.FindElement(By.XPath("//input[@value='Yes']"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;

        public void ClickNo()
        {
            NoElement.Click();
            Thread.Sleep(2000);
        }
        public void ClickYes()
        {
            YesElement.Click();
            Thread.Sleep(2000);
        }
    }
}
