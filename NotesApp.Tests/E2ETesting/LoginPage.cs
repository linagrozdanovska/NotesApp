using OpenQA.Selenium;

namespace NotesApp.Tests.E2ETesting
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        public LoginPage(IWebDriver driver) => _driver = driver;
        private const string URI = "https://localhost:44337/Identity/Account/Login";

        private IWebElement EmailElement => _driver.FindElement(By.Id("Input_Email"));
        private IWebElement PasswordElement => _driver.FindElement(By.Id("Input_Password"));
        private IWebElement LogInElement => _driver.FindElement(By.Id("login-submit"));
        public string Title => _driver.Title;
        public string Source => _driver.PageSource;

        public void PopulateEmail(string email)
        {
            EmailElement.SendKeys(email);
        }
        public void PopulatePassword(string password)
        {
            PasswordElement.SendKeys(password);
        }
        public void ClickLogIn()
        {
            LogInElement.Click();
        }
        public void Navigate() => _driver.Navigate().GoToUrl(URI);
    }
}
