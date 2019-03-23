using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class BasicGetTests
    {

        private string path = "/api/BasicAuthSchemeTest";

        private TestServer apiServer;

        [SetUp]
        public void Setup()
        {
            this.apiServer = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>());
        }

        [TearDown]
        public void TearDown()
        {
            this.apiServer.Dispose();
        }

        [Test]
        public async Task Basic_Get_ValidUser_ReturnsContent()
        {
            string username = "test";
            string password = "123";

            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .GetAsync();

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.IsNotEmpty(responseContent);
        }

        [Test]
        public async Task Basic_Get_ValidUser_returnsOK()
        {
            string username = "test";
            string password = "123";

            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }


        [Test]
        public async Task Basic_Get_InvalidUser_returnsUnauthorized()
        {
            string username = "test";
            string password = "invalid_password";

            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Get_NoHeather_returnsUnauthorized()
        {
            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                //.AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Get_NoHeather_returnsUnauthorized_2()
        {
            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Get_ValidatingUserReturnsError_returnsConfiguredStatus()
        {
            const string userConfiguredToCauseError = "testError";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.RequestTimeout;

            string password = "123";

            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(userConfiguredToCauseError, password))
                .GetAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }


        [Test]
        public async Task Basic_Get_ValidatingKeyThrowsException_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "exception";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.BadRequest;


            string password = "invalid_password";

            var response = await apiServer.CreateRequest(path)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(keyConfiguredToCauseError, password))
                .GetAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }






    }
}