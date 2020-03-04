using System;
using System.Text.Json;
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests
{
    public class AuthenticationTests : BaseTest, IAsyncLifetime
    {
        private IWebHost _host;

        public AuthenticationTests() : base(runHeadless: false)
        {
            _host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(x => { x.AddTransient<IConfigureOptions<GraphiQlOptions>,GraphiQlTestOptionsSetup>(); })
                .UseStartup<Startup>()
                .UseKestrel()
                .UseUrls("http://*:5001")
                .Build();
        }

        [Fact]
        public async Task RequiresAuthentication()
        {
            // Arrange + Act
            var result = string.Empty;
            await RunTest( async driver =>
            {
                Driver.Navigate().GoToUrl("http://localhost:5001/graphql");
                await Task.Delay(500);
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
        
        private static void RequireAuthMap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("This page requires authentication");
            });
        }
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