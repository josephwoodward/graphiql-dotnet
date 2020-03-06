using System;
using System.Text.Json;
using System.Threading;
using GraphiQl.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace GraphiQl.Tests
{
    public class BasicTest : SeleniumTest, IClassFixture<HostFixture>
    {
        private readonly HostFixture _fixture;

        public BasicTest(HostFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CanQueryGraphQl()
        {
            // TODO: Use PageModel

            // Arrange
            var result = string.Empty;
            var query = @"{hero{id,name}}";
                
            // Act
            RunTest( driver =>
            {
                driver.Navigate().GoToUrl(_fixture.GraphiQlUri + Uri.EscapeDataString(query));
                var button = driver.FindElementByClassName("execute-button");
                button?.Click();
                
                //TODO: https://www.selenium.dev/documentation/en/webdriver/waits/
                Thread.Sleep(2000);

                // UGH!
                result = driver.FindElementByClassName("result-window").Text
                    .Replace("\n", "")
                    .Replace(" ", "");
            });

            // Assert
            using var channelResponse = JsonDocument.Parse(result);
            var data = channelResponse.RootElement.GetProperty("data");

            data.GetProperty("hero").GetProperty("name").GetString().ShouldBe("R2-D2");
        }
    }
}