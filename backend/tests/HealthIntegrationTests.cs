using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Medicare.Tests
{
    public record HealthDto(string status, System.DateTime timestamp);

    public class HealthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HealthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetHealthEndpoint_Returns_Healthy_Status()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var dto = await response.Content.ReadFromJsonAsync<HealthDto>();
            dto.Should().NotBeNull();
            dto!.status.Should().Be("healthy");
            dto.timestamp.Should().BeBefore(System.DateTime.UtcNow.AddSeconds(5));
        }
    }
}
