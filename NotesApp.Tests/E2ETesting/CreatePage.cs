using OpenQA.Selenium;
using System.Threading;

namespace NotesApp.Tests.E2ETesting
{
    public class CreatePage
    {
        private readonly IWebDriver _driver;

        public CreatePage(IWebDriver driver) => _driver = driver;
        private const string URI = "https://localhost:44337/Notes/Create";

        private IWebElement TitleElement => _driver.FindElement(By.Id("Title"));
        private IWebElement BodyElement => _driver.FindElement(By.Id("Body"));
        private IWebElement CreateElement => _driver.FindElement(By.Id("create"));

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

        public void ClickCreate()
        {
            CreateElement.Click();
            Thread.Sleep(2000);
        }

        public void Navigate()
        {
            _driver.Navigate().GoToUrl(URI);
            Thread.Sleep(1000);
        }
    }
}
