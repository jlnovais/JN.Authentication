using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class ApiKeyQueryStringNotAllowedTests
    {

        private TestServer _apiServer;

        [SetUp]
        public void Setup()
        {
            this._apiServer = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>().UseSetting("ApiKeyAcceptsQueryString", "false")
            );
        }

        [TearDown]
        public void TearDown()
        {
            this._apiServer.Dispose();
        }

        private readonly string path = "/api/ApiKeyAuthSchemeTest";
        private readonly string pathNoGet = "/api/ApiKeyAuthSchemeNoGetTest";


        [Test]
        public async Task ApiKey_GetQueryStringNotAccepted_ValidKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(pathNoGet + "?ApiKey=123")
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }


        [Test]
        public async Task ApiKey_GetQueryStringNotAccepted_InvalidKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(path + "?ApiKey=invalid_key")
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_GetQueryStringNotAccepted_NoKey_returnsUnauthorized()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(path)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApiKey_GetQueryStringNotAccepted_NoKey_returnsUnauthorized_2()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(path + "?ApiKey=")
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }




    }
}