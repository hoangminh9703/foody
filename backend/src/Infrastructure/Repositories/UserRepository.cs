using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Medicare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MedicareDbContext _dbContext;

        public UserRepository(MedicareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return _dbContext.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail, cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}