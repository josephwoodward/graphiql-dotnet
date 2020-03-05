using System.Threading;
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests
{
    public class AuthenticationTests : BaseTest, IAsyncLifetime
    {
        private readonly IWebHost _host;

        public AuthenticationTests()
        {
            _host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(x => { x.AddTransient<IConfigureOptions<GraphiQlOptions>,GraphiQlTestOptionsSetup>(); })
                .UseStartup<Startup>()
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
                Driver.Navigate().GoToUrl("http://localhost:5001/graphql");
                Thread.Sleep(2000);
                result = Driver.PageSource;
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

        internal class GraphiQlTestOptionsSetup : IConfigureOptions<GraphiQlOptions>
        {
            public void Configure(GraphiQlOptions options)
            {
                options.IsAuthenticated = context =>
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 400;
                    context.Response.WriteAsync("This page requires authentication");

                    return Task.FromResult(false);
                };
            }
        }
    }
}