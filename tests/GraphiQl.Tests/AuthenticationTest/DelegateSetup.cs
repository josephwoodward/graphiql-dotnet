using System;
using System.Threading;
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests.AuthenticationTest
{
    public class CustomStartup : Startup
    {
        public CustomStartup(IConfiguration configuration) : base(configuration) {}

        public override void ConfigureGraphQl(IServiceCollection services)
            => services.AddGraphiQl(x => x.IsAuthenticated = context =>
            {
                context.Response.Clear();
                context.Response.StatusCode = 400;
                context.Response.WriteAsync("This page requires authentication");

                return Task.FromResult(false);
            });
    }

    public class DelegateSetup : SeleniumTest, IAsyncLifetime
    {
        private readonly IWebHost _host;

        public DelegateSetup()
        {
            _host = WebHost.CreateDefaultBuilder()
                .UseStartup<CustomStartup>()
                .UseKestrel()
                .UseUrls("http://*:5001")
                .Build();
        }

        [Fact]
        public void RequiresAuthentication()
        {
            // Arrange + Act
            var result = string.Empty;
            RunTest(driver =>
            {
                driver.Navigate().GoToUrl("http://localhost:5001/graphql");

                driver.Manage()
                    .Timeouts()
                    .ImplicitWait = TimeSpan.FromSeconds(2);

                result = driver.PageSource;
            });

            // Assert
            result.ShouldContain("This page requires authentication");
        }

        public async Task InitializeAsync()
            => await _host.StartAsync().ConfigureAwait(false);

        public Task DisposeAsync()
        {
            _host.Dispose();
            return Task.CompletedTask;
        }
    }
}