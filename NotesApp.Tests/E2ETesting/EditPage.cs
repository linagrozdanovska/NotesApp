using OpenQA.Selenium;
using System.Threading;

namespace NotesApp.Tests.E2ETesting
{
    public class EditPage
    {
        private readonly IWebDriver _driver;

        public EditPage(IWebDriver driver) => _driver = driver;

        private IWebElement TitleElement => _driver.FindElement(By.Id("Title"));
        private IWebElement BodyElement => _driver.FindElement(By.Id("Body"));
        private IWebElement SaveElement => _driver.FindElement(By.Id("save"));

        public string Title => _driver.Title;
        public string Source => _driver.PageSource;
        public string TitleErrorMessage => _driver.FindElement(By.Id("Title-error")).Text;
        public string BodyErrorMessage => _driver.FindElement(By.Id("Body-error")).Text;

        public void PopulateTitle(string title)
        {
            TitleElement.SendKeys(title);
            Thread.Sleep(1000);
        }

        public void PopulateBody(string body)
        {
            BodyElement.SendKeys(body);
            Thread.Sleep(1000);
        }

        public void ClearTitle()
        {
            TitleElement.Clear();
            Thread.Sleep(1000);
        }

        public void ClearBody()
        {
            BodyElement.Clear();
            Thread.Sleep(1000);
        }

        public void ClickSave()
        {
            SaveElement.Click();
            Thread.Sleep(2000);
        }
    }
}
