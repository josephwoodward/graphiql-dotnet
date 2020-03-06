using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests.OverrideGraphiQlPathTests
{
    public class ConfigureOptionsSetup : SeleniumTest, IAsyncLifetime
    {
        private readonly IWebHost _host;

        public ConfigureOptionsSetup()
        {
            _host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddTransient<IConfigureOptions<GraphiQlOptions>, GraphiQlTestOptionsSetup>();
                })
                .UseStartup<Startup>()
                .UseKestrel()
                .UseUrls("http://*:5001")
                .Build();
        }

        public async Task InitializeAsync()
            => await _host.StartAsync().ConfigureAwait(false);

        [Fact]
        public void CanOverrideGraphiQlPath()
        {
            // TODO: Use PageModel

            // Arrange
            var result = string.Empty;
            var query = @"{hero{id,name}}";
                
            // Act
            RunTest( driver =>
            {
                driver.Navigate().GoToUrl($"http://localhost:5001{Startup.CustomGraphQlPath}?query=" + Uri.EscapeDataString(query));
                var button = driver.FindElementByClassName("execute-button");
                button?.Click();
                
                Thread.Sleep(2000);

                // UGH!
                result = driver
                    .FindElementByClassName("result-window").Text
                    .Replace("\n", "")
                    .Replace(" ", "");
            });

            // Assert
            using var channelResponse = JsonDocument.Parse(result);
            var data = channelResponse.RootElement.GetProperty("data");

            data.GetProperty("hero").GetProperty("name").GetString().ShouldBe("R2-D2");
        }

        public async Task DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        internal class GraphiQlTestOptionsSetup : IConfigureOptions<GraphiQlOptions>
        {
            public void Configure(GraphiQlOptions options)
            {
                options.GraphiQlPath = Startup.CustomGraphQlPath;
            }
        }
    }
}