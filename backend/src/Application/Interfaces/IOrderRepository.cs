using Medicare.Application.DTOs;
using Medicare.Domain.Entities;

namespace Medicare.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<OrderRequest>> GetPagedAsync(OrderFilterRequest filter, CancellationToken cancellationToken = default);
        Task<OrderRequest> AddAsync(OrderRequest order, CancellationToken cancellationToken = default);
        Task UpdateAsync(OrderRequest order, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}