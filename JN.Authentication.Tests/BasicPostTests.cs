using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class BasicPostTests
    {

        private readonly string path = "/api/BasicAuthSchemeTest";

        private TestServer _apiServer;

        [SetUp]
        public void Setup()
        {
            _apiServer = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
            );
        }

        [TearDown]
        public void TearDown()
        {
            this._apiServer.Dispose();
        }

        [Test]
        public async Task Basic_Post_ValidUser_ReturnsContent()
        {
            var content = Tools.GetContent();

            string username = "test";
            string password = "123";

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.IsNotEmpty(responseContent);
        }

        [Test]
        public async Task Basic_Post_ValidUser_returnsOK()
        {
            var content = Tools.GetContent();

            string username = "test";
            string password = "123";

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }


        [Test]
        public async Task Basic_Post_InvalidUser_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            string username = "test";
            string password = "invalid_password";

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Post_NoHeather_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                //.AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Post_NoHeather_returnsUnauthorized_2()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "")
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Basic_Post_ValidatingUserReturnsError_returnsConfiguredStatus()
        {
            const string userConfiguredToCauseError = "testError";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.RequestTimeout;

            var content = Tools.GetContent();

            string password = "123";

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(userConfiguredToCauseError, password))
                .PostAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }


        [Test]
        public async Task Basic_Post_ValidatingKeyThrowsException_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "exception";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.BadRequest;

            var content = Tools.GetContent();

            string password = "invalid_password";

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(keyConfiguredToCauseError, password))
                .PostAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }






    }
}