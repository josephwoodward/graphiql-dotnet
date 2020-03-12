using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GraphiQl.Demo;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests.OverrideGraphQlPathTests
{
    public class CustomStartup : Startup
    {
        public CustomStartup(IConfiguration configuration) : base(configuration) {}

        public override void ConfigureGraphQl(IServiceCollection services) 
            => services.AddGraphiQl(x => x.GraphQlApiPath = CustomGraphQlPath);
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

        public async Task InitializeAsync()
            => await _host.StartAsync().ConfigureAwait(false);

        [Fact]
        public void CanOverrideGraphQlPath()
        {
            // TODO: Use PageModel

            // Arrange
            var result = string.Empty;
            var query = @"{hero{id,name}}";
                
            // Act
            RunTest( driver =>
            {
                driver.Navigate().GoToUrl("http://localhost:5001/graphql?query=" + Uri.EscapeDataString(query));
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
    }
}