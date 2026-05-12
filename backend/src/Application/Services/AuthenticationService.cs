using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Medicare.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IUserRepository userRepository, ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return FailedResult("Invalid email or password");
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !user.IsActive)
            {
                _logger.LogWarning("Failed authentication attempt for email {Email}", normalizedEmail);
                return FailedResult("Invalid email or password");
            }

            var isValidPassword = await ValidatePasswordAsync(password, user.PasswordHash);
            if (!isValidPassword)
            {
                _logger.LogWarning("Failed authentication attempt for email {Email}", normalizedEmail);
                return FailedResult("Invalid email or password");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Successful login for user {UserId} ({Email})", user.Id, user.Email);

            return new AuthResult
            {
                Success = true,
                Message = "Login successful",
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                LastLoginAt = user.LastLoginAt,
                ExpiresAt = DateTime.UtcNow.Add(SessionTimeout),
            };
        }

        public Task<bool> ValidatePasswordAsync(string password, string hash)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        }

        private static AuthResult FailedResult(string message)
        {
            return new AuthResult
            {
                Success = false,
                Message = message,
                ExpiresAt = DateTime.UtcNow,
            };
        }
    }
}