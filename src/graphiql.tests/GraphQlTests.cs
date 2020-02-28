using System;
using System.Text.Json;
using System.Threading.Tasks;
using graphiql.tests.Fixtures;
using Shouldly;
using Xunit;

namespace GraphiQl.tests
{
    public class GraphQlTests : BaseTest, IClassFixture<HostFixture>
    {
        private readonly HostFixture _fixture;

        public GraphQlTests(HostFixture fixture) : base(runHeadless: true)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CanQueryGraphQl()
        {
            // TODO: Use PageModel

            // Arrange
            var result = string.Empty;
            var query = @"{hero{id,name}}";
                
            // Act
            await RunTest(async driver =>
            {
                Driver.Navigate().GoToUrl(_fixture.GraphiQlUri + Uri.EscapeDataString(query));
                var button = Driver.FindElementByClassName("execute-button");
                button?.Click();

                await Task.Delay(2000);

                // UGH!
                result = Driver
                    .FindElementByClassName("result-window").Text
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