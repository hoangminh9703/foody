using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Medicare.Tests
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IUserRepository>();
                    services.PostConfigure<SessionOptions>(options =>
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    });
                    services.AddSingleton<IUserRepository>(new FakeUserRepository(new User
                    {
                        Id = 1,
                        Email = "admin@medicare.local",
                        FullName = "System Administrator",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        Role = UserRole.Admin,
                        IsActive = true,
                    }));
                });
            });
        }

        [Fact]
        public async Task Login_Me_And_Logout_Flow_Works_With_Session()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
                AllowAutoRedirect = false,
            });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "admin@medicare.local",
                password = "Admin123!",
            });

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginPayload = await loginResponse.Content.ReadFromJsonAsync<AuthPayload>();
            loginPayload.Should().NotBeNull();
            loginPayload!.Success.Should().BeTrue();
            loginPayload.Email.Should().Be("admin@medicare.local");

            var currentUserResponse = await client.GetAsync("/api/auth/me");
            currentUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<AuthPayload>();
            currentUser.Should().NotBeNull();
            currentUser!.FullName.Should().Be("System Administrator");

            var logoutResponse = await client.PostAsync("/api/auth/logout", null);
            logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var unauthorizedResponse = await client.GetAsync("/api/auth/me");
            unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private sealed class AuthPayload
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public int UserId { get; set; }
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int Role { get; set; }
            public DateTime? LastLoginAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}