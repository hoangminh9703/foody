using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Medicare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly MedicareDbContext _dbContext;

        public MenuRepository(MedicareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Menu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Menus
                .Include(menu => menu.Items)
                .FirstOrDefaultAsync(menu => menu.Id == id, cancellationToken);
        }

        public async Task<PagedResult<Menu>> GetPagedAsync(MenuFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var query = _dbContext.Menus
                .Include(menu => menu.Items)
                .AsNoTracking()
                .AsQueryable();

            if (filter.DateFrom.HasValue)
            {
                var dateFrom = filter.DateFrom.Value.Date;
                query = query.Where(menu => menu.DateApplied.Date >= dateFrom);
            }

            if (filter.DateTo.HasValue)
            {
                var dateTo = filter.DateTo.Value.Date;
                query = query.Where(menu => menu.DateApplied.Date <= dateTo);
            }

            if (filter.MealType.HasValue)
            {
                var mealType = filter.MealType.Value;
                query = query.Where(menu => menu.MealType == mealType);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(menu => menu.DateApplied)
                .ThenBy(menu => menu.MealType)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Menu>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<Menu> AddAsync(Menu menu, CancellationToken cancellationToken = default)
        {
            await _dbContext.Menus.AddAsync(menu, cancellationToken);
            return menu;
        }

        public Task UpdateAsync(Menu menu, CancellationToken cancellationToken = default)
        {
            _dbContext.Menus.Update(menu);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Menu menu, CancellationToken cancellationToken = default)
        {
            _dbContext.Menus.Remove(menu);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(DateTime date, MealType mealType, int? excludeMenuId = null, CancellationToken cancellationToken = default)
        {
            var normalizedDate = date.Date;

            return _dbContext.Menus.AnyAsync(
                menu => menu.DateApplied.Date == normalizedDate
                    && menu.MealType == mealType
                    && (!excludeMenuId.HasValue || menu.Id != excludeMenuId.Value),
                cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
