using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class ApiKeyPostTests
    {

        private string path = "/api/ApiKeyAuthSchemeTest/MyTest";

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
        public async Task Anonymous_returnsOK()
        {
            var response = await apiServer.CreateClient().GetAsync("/api/values");

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task ApiKey_Post_ValidKey_ReturnsContent()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .PostAsync();

            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.IsNotEmpty(responseContent);
        }

        [Test]
        public async Task ApiKey_Post_ValidKey_returnsOK()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .PostAsync();


            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }


        [Test]
        public async Task ApiKey_Post_InvalidKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "invalid_key")
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Post_NoKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                //.AddHeader("ApiKey", "invalid_key")
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Post_NoKey_returnsUnauthorized_2()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "")
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_Post_ValidatingKeyReturnsError_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "1234";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.RequestTimeout;

            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", keyConfiguredToCauseError)
                .PostAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }


        [Test]
        public async Task ApiKey_Post_ValidatingKeyThrowsException_returnsConfiguredStatus()
        {
            const string keyConfiguredToCauseError = "exception";
            var statusConfiguredInAppForValidatingKeyErrors = HttpStatusCode.BadRequest;

            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", keyConfiguredToCauseError)
                .PostAsync();

            Assert.That(response.StatusCode == statusConfiguredInAppForValidatingKeyErrors);
        }






    }
}