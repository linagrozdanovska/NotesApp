using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace NotesApp.Tests.IntegrationTesting
{
    public class IntegrationTests : IntegrationTestBase
    {
        public IntegrationTests(MediaGalleryFactory<FakeStartup> factory) : base(factory)
        {

        }

        [Fact]
        public async Task Home_UnauthenicatedUser_ShowsWelcomeScreen()
        {
            // Arrange
            var client = GetFactory().CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                }
            );

            // Act
            var response = await client.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Contains("Welcome", body);
        }

        [Theory]
        [InlineData("/Notes/Index")]
        [InlineData("/Notes/Details/3")]
        [InlineData("/Notes/Create")]
        [InlineData("/Notes/Edit/3")]
        [InlineData("/Notes/Delete/3")]
        public async Task AuthorizedMethod_UnauthenticatedUser_RedirectsToLogin(string url)
        {
            // Arrange
            var client = GetFactory().CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                }
            );

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login", response.Headers.Location.OriginalString);
        }

        [Theory]
        [InlineData("/Notes/Index")]
        [InlineData("/Notes/Details/3")]
        [InlineData("/Notes/Create")]
        [InlineData("/Notes/Edit/3")]
        [InlineData("/Notes/Delete/3")]
        public async Task AuthorizedMethod_AuthenticatedUser_ReturnStatusCodeOKAndCorrectContentType(string url)
        {
            // Arrange
            var client = GetFactory(hasUser: true).CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

    }
}
