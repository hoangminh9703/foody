using FluentAssertions;
using Medicare.Application.Services;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Medicare.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ReturnsSuccess_AndUpdatesLastLoginAt()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "admin@medicare.local",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
            };
            var repository = new FakeUserRepository(user);
            var service = new AuthenticationService(repository, NullLogger<AuthenticationService>.Instance);

            // Act
            var result = await service.LoginAsync("admin@medicare.local", "Admin123!");

            // Assert
            result.Success.Should().BeTrue();
            result.UserId.Should().Be(1);
            result.Email.Should().Be("admin@medicare.local");
            result.FullName.Should().Be("System Administrator");
            result.LastLoginAt.Should().NotBeNull();
            repository.Users.Should().ContainSingle().Which.LastLoginAt.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_ForInvalidPassword()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "admin@medicare.local",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
            };
            var repository = new FakeUserRepository(user);
            var service = new AuthenticationService(repository, NullLogger<AuthenticationService>.Instance);

            // Act
            var result = await service.LoginAsync("admin@medicare.local", "wrong-password");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid email or password");
        }
    }

    internal sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(params User[] users)
        {
            _users = users.ToList();
        }

        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return Task.FromResult(_users.FirstOrDefault(user => user.Email.ToLowerInvariant() == normalizedEmail));
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var existingIndex = _users.FindIndex(existing => existing.Id == user.Id);
            if (existingIndex >= 0)
            {
                _users[existingIndex] = user;
            }
            else
            {
                _users.Add(user);
            }

            return Task.CompletedTask;
        }
    }
}