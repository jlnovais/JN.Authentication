using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using JN.Authentication.APITest;
using NUnit.Framework;

namespace JN.Authentication.Tests
{
    public class PostOnlyTests
    {

        private TestServer _apiServer;

        [SetUp]
        public void Setup()
        {
            _apiServer = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>().UseSetting("HttpPostMethodOnly", "true")
            );
        }

        [TearDown]
        public void TearDown()
        {
            this._apiServer.Dispose();
        }

        private readonly string pathApiKey = "/api/ApiKeyAuthSchemeTest/MyTest";
        private readonly string pathBasic = "/api/BasicAuthSchemeTest";
        private readonly string pathBasicPostOnly = "/api/BasicAuthSchemePostOnlyTest";



        [Test]
        public async Task ApiKeyPostOnly_Get_ValidKey_returnsMethodNotAllowed()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(pathBasicPostOnly)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task ApiKeyPostOnly_Post_ValidKey_returnsOK()
        {
            var content = Tools.GetContent();

            var response = await _apiServer.CreateRequest(pathApiKey)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task BasicPostOnly_Get_ValidUser_returnsMethodNotAllowed()
        {
            string username = "test";
            string password = "123";

            var response = await _apiServer.CreateRequest(pathBasicPostOnly)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .GetAsync();

            Assert.That(response.StatusCode == HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public async Task BasicPostOnly_Post_ValidUser_returnsOK()
        {
            var content = Tools.GetContent();

            string username = "test";
            string password = "123";

            var response = await _apiServer.CreateRequest(pathBasic)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }





    }
}