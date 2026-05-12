using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Medicare.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ILogger<MenuService> _logger;

        public MenuService(IMenuRepository menuRepository, ILogger<MenuService> logger)
        {
            _menuRepository = menuRepository;
            _logger = logger;
        }

        public async Task<MenuDto> CreateMenuAsync(CreateMenuRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCreateOrUpdateRequest(request.DateApplied, request.Items);

            if (await MenuExistsForDateAndMealTypeAsync(request.DateApplied, request.MealType, null, cancellationToken))
            {
                throw new InvalidOperationException("Menu already exists for this date and meal type.");
            }

            var menu = new Menu
            {
                DateApplied = request.DateApplied.Date,
                MealType = request.MealType,
                Description = request.Description.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = request.Items
                    .OrderBy(item => item.DisplayOrder)
                    .Select(item => new MenuItem
                    {
                        Name = item.Name.Trim(),
                        Description = item.Description.Trim(),
                        Price = item.Price,
                        DisplayOrder = item.DisplayOrder,
                        CreatedAt = DateTime.UtcNow,
                    })
                    .ToList(),
            };

            await _menuRepository.AddAsync(menu, cancellationToken);
            await _menuRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created menu {MenuId} for {DateApplied} ({MealType})", menu.Id, menu.DateApplied, menu.MealType);

            return MapToMenuDto(menu);
        }

        public async Task<MenuDto> GetMenuByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
            if (menu is null)
            {
                throw new KeyNotFoundException($"Menu with id {id} was not found.");
            }

            return MapToMenuDto(menu);
        }

        public async Task<PagedResult<MenuDto>> GetMenusAsync(MenuFilterRequest filter, CancellationToken cancellationToken = default)
        {
            if (filter.DateFrom.HasValue && filter.DateTo.HasValue && filter.DateFrom.Value.Date > filter.DateTo.Value.Date)
            {
                throw new ArgumentException("dateFrom must be earlier than or equal to dateTo.");
            }

            var result = await _menuRepository.GetPagedAsync(filter, cancellationToken);
            return new PagedResult<MenuDto>
            {
                Items = result.Items.Select(MapToMenuDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize,
            };
        }

        public async Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCreateOrUpdateRequest(request.DateApplied, request.Items);

            var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
            if (menu is null)
            {
                throw new KeyNotFoundException($"Menu with id {id} was not found.");
            }

            EnsureMenuNotInPast(menu.DateApplied);

            if (await MenuExistsForDateAndMealTypeAsync(request.DateApplied, request.MealType, id, cancellationToken))
            {
                throw new InvalidOperationException("Menu already exists for this date and meal type.");
            }

            menu.DateApplied = request.DateApplied.Date;
            menu.MealType = request.MealType;
            menu.Description = request.Description.Trim();
            menu.UpdatedAt = DateTime.UtcNow;

            menu.Items.Clear();
            foreach (var item in request.Items.OrderBy(item => item.DisplayOrder))
            {
                menu.Items.Add(new MenuItem
                {
                    Name = item.Name.Trim(),
                    Description = item.Description.Trim(),
                    Price = item.Price,
                    DisplayOrder = item.DisplayOrder,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            await _menuRepository.UpdateAsync(menu, cancellationToken);
            await _menuRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated menu {MenuId}", menu.Id);

            return MapToMenuDto(menu);
        }

        public async Task DeleteMenuAsync(int id, CancellationToken cancellationToken = default)
        {
            var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
            if (menu is null)
            {
                throw new KeyNotFoundException($"Menu with id {id} was not found.");
            }

            EnsureMenuNotInPast(menu.DateApplied);

            await _menuRepository.DeleteAsync(menu, cancellationToken);
            await _menuRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted menu {MenuId}", menu.Id);
        }

        public Task<bool> MenuExistsForDateAndMealTypeAsync(DateTime date, MealType mealType, int? excludeMenuId = null, CancellationToken cancellationToken = default)
        {
            return _menuRepository.ExistsAsync(date, mealType, excludeMenuId, cancellationToken);
        }

        private static MenuDto MapToMenuDto(Menu menu)
        {
            return new MenuDto
            {
                Id = menu.Id,
                DateApplied = menu.DateApplied,
                MealType = menu.MealType.ToString(),
                Description = menu.Description,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt,
                Items = menu.Items
                    .OrderBy(item => item.DisplayOrder)
                    .Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        DisplayOrder = item.DisplayOrder,
                    })
                    .ToList(),
            };
        }

        private static void ValidateCreateOrUpdateRequest(DateTime dateApplied, IEnumerable<MenuItemRequest> items)
        {
            if (dateApplied.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Cannot create or update menu for past dates.");
            }

            var menuItems = items?.ToList() ?? new List<MenuItemRequest>();
            if (!menuItems.Any())
            {
                throw new ArgumentException("Menu must contain at least one item.");
            }

            foreach (var item in menuItems)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    throw new ArgumentException("Each menu item must have a name.");
                }

                if (item.Price < 0)
                {
                    throw new ArgumentException("Item price must be greater than or equal to 0.");
                }

                if (decimal.Round(item.Price, 2) != item.Price)
                {
                    throw new ArgumentException("Item price can have at most 2 decimal places.");
                }
            }
        }

        private static void EnsureMenuNotInPast(DateTime menuDate)
        {
            if (menuDate.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Cannot modify or delete menu for past dates.");
            }
        }
    }
}
