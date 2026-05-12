using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Medicare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MedicareDbContext _dbContext;

        public OrderRepository(MedicareDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.OrderRequests
                .Include(order => order.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task<PagedResult<OrderRequest>> GetPagedAsync(OrderFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var query = _dbContext.OrderRequests
                .Include(order => order.Items)
                .AsNoTracking()
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                var status = filter.Status.Value;
                query = query.Where(order => order.Status == status);
            }

            if (filter.DateFrom.HasValue)
            {
                var dateFrom = filter.DateFrom.Value.Date;
                query = query.Where(order => order.OrderDate.Date >= dateFrom);
            }

            if (filter.DateTo.HasValue)
            {
                var dateTo = filter.DateTo.Value.Date;
                query = query.Where(order => order.OrderDate.Date <= dateTo);
            }

            if (filter.MealType.HasValue)
            {
                var mealType = filter.MealType.Value;
                query = query.Where(order => order.MealType == mealType);
            }

            if (!string.IsNullOrWhiteSpace(filter.CustomerName))
            {
                var customerName = filter.CustomerName.Trim().ToLowerInvariant();
                query = query.Where(order => order.CustomerName.ToLower().Contains(customerName));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(order => order.OrderDate)
                .ThenByDescending(order => order.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<OrderRequest>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<OrderRequest> AddAsync(OrderRequest order, CancellationToken cancellationToken = default)
        {
            await _dbContext.OrderRequests.AddAsync(order, cancellationToken);
            return order;
        }

        public Task UpdateAsync(OrderRequest order, CancellationToken cancellationToken = default)
        {
            _dbContext.OrderRequests.Update(order);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}