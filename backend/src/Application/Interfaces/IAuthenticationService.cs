using Medicare.Application.DTOs;
using Medicare.Domain.Entities;

namespace Medicare.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<bool> ValidatePasswordAsync(string password, string hash);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}