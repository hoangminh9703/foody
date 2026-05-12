using Medicare.Application.DTOs;
using Medicare.Domain.Entities;

namespace Medicare.Application.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilterRequest filter, CancellationToken cancellationToken = default);
        Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
        Task<bool> CanTransitionStatusAsync(OrderStatus from, OrderStatus to, CancellationToken cancellationToken = default);
        Task<bool> IsMenuAvailableAsync(DateTime date, MealType mealType, CancellationToken cancellationToken = default);
    }
}