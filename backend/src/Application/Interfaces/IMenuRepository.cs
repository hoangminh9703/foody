using Medicare.Application.DTOs;
using Medicare.Domain.Entities;

namespace Medicare.Application.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<Menu>> GetPagedAsync(MenuFilterRequest filter, CancellationToken cancellationToken = default);
        Task<Menu> AddAsync(Menu menu, CancellationToken cancellationToken = default);
        Task UpdateAsync(Menu menu, CancellationToken cancellationToken = default);
        Task DeleteAsync(Menu menu, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(DateTime date, MealType mealType, int? excludeMenuId = null, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
