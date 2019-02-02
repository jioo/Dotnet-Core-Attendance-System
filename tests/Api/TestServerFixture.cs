using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using WebApi;
using WebApi.Constants;
using WebApi.Infrastructure;

namespace Test.Api
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }
        public ApplicationDbContext Context { get; }
        public Roles Roles { get; }

        public TestServerFixture()
        {
            var builder = new WebHostBuilder()
                   .UseEnvironment("Test")
                   .UseStartup<Startup>();

            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();
            Context = _testServer.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            Roles = new Roles();
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}