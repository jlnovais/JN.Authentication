using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class ApiKeyGetTests
    {

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

        private string path = "/api/ApiKeyAuthSchemeTest";

        [Test]
        public async Task ApiKey_Get_ValidKey_ReturnsContent()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .GetAsync();

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.IsNotEmpty(responseContent);
        }

        [Test]
        public async Task ApiKey_Get_ValidKey_returnsOK()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }


        [Test]
        public async Task ApiKey_Get_InvalidKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "invalid_key")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Get_NoKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                //.AddHeader("ApiKey", "invalid_key")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Get_NoKey_returnsUnauthorized_2()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Get_ValidatingKeyReturnsError_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "1234";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.RequestTimeout;

            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", keyConfiguredToCauseError)
                .GetAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }


        [Test]
        public async Task ApiKey_Get_ValidatingKeyThrowsException_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "exception";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.BadRequest;

            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", keyConfiguredToCauseError)
                .GetAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }





    }
}