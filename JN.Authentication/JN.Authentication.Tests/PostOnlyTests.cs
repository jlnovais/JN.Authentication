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

        private TestServer apiServer;

        [SetUp]
        public void Setup()
        {
            this.apiServer = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>().UseSetting("HttpPostMethodOnly","true")
            );
        }

        [TearDown]
        public void TearDown()
        {
            this.apiServer.Dispose();
        }

        private string pathApiKey = "/api/ApiKeyAuthSchemeTest/MyTest";
        private string pathBasic = "/api/BasicAuthSchemeTest";



        [Test]
        public async Task ApiKeyPostOnly_Get_ValidKey_returnsMethodNotAllowed()
        {
            var content = Tools.GetContent();

            var response = await apiServer.CreateRequest(pathApiKey)
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

            var response = await apiServer.CreateRequest(pathApiKey)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("ApiKey", "123")
                .PostAsync();

            var y = response.StatusCode;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task BasicPostOnly_Get_ValidUser_returnsMethodNotAllowed()
        {
            string username = "test";
            string password = "123";

            var response = await apiServer.CreateRequest(pathBasic)
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

            var response = await apiServer.CreateRequest(pathBasic)
                .And(x => x.Content = content)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Authorization", "Basic " + Tools.BasicAuthCredentials(username, password))
                .PostAsync();

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }
    




    }
}