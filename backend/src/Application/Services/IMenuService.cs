using Medicare.Application.DTOs;
using Medicare.Domain.Entities;

namespace Medicare.Application.Services
{
    public interface IMenuService
    {
        Task<MenuDto> CreateMenuAsync(CreateMenuRequest request, CancellationToken cancellationToken = default);
        Task<MenuDto> GetMenuByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<MenuDto>> GetMenusAsync(MenuFilterRequest filter, CancellationToken cancellationToken = default);
        Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuRequest request, CancellationToken cancellationToken = default);
        Task DeleteMenuAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> MenuExistsForDateAndMealTypeAsync(DateTime date, MealType mealType, int? excludeMenuId = null, CancellationToken cancellationToken = default);
    }
}
