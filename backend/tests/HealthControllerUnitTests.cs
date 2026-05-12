using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using Medicare.Api.Controllers;

namespace Medicare.Tests
{
    public class HealthControllerUnitTests
    {
        [Fact]
        public void GetHealth_Returns_Healthy_Status_And_Timestamp()
        {
            // Arrange
            var controller = new HealthController();

            // Act
            var actionResult = controller.GetHealth();

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var ok = actionResult as OkObjectResult;
            ok!.Value.Should().NotBeNull();

            var value = ok.Value!;
            var type = value.GetType();

            var statusProp = type.GetProperty("status");
            statusProp.Should().NotBeNull();
            var status = statusProp!.GetValue(value) as string;
            status.Should().Be("healthy");

            var tsProp = type.GetProperty("timestamp");
            tsProp.Should().NotBeNull();
            var tsVal = tsProp!.GetValue(value);
            tsVal.Should().BeOfType<DateTime>();
        }
    }
}
