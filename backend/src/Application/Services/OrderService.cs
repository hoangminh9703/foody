using System.Text.RegularExpressions;
using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;
using Medicare.Domain.Entities;
using Medicare.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Medicare.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IMenuRepository menuRepository, IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _menuRepository = menuRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCreateOrderRequest(request);

            var menu = await LoadMenuForOrderAsync(request.OrderDate, request.MealType, cancellationToken);
            var menuItems = menu.Items.ToDictionary(item => item.Id);

            var order = new OrderRequest
            {
                CustomerName = request.CustomerName.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                OrderDate = request.OrderDate.Date,
                MealType = request.MealType,
                Notes = request.Notes?.Trim() ?? string.Empty,
                Status = OrderStatus.New,
                CreatedAt = DateTime.UtcNow,
            };

            var totalQuantity = 0;
            var totalPrice = 0m;

            foreach (var item in request.Items)
            {
                if (!menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                {
                    throw new ArgumentException($"Menu item {item.MenuItemId} was not found in the selected menu.");
                }

                var subtotal = decimal.Round(menuItem.Price * item.Quantity, 2, MidpointRounding.AwayFromZero);

                order.Items.Add(new OrderRequestItem
                {
                    MenuItemId = menuItem.Id,
                    MenuItemName = menuItem.Name,
                    Quantity = item.Quantity,
                    UnitPrice = menuItem.Price,
                    Subtotal = subtotal,
                    SpecialRequest = string.IsNullOrWhiteSpace(item.SpecialRequest) ? null : item.SpecialRequest.Trim(),
                    CreatedAt = DateTime.UtcNow,
                });

                totalQuantity += item.Quantity;
                totalPrice += subtotal;
            }

            order.TotalQuantity = totalQuantity;
            order.TotalPrice = decimal.Round(totalPrice, 2, MidpointRounding.AwayFromZero);

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created order {OrderId} for {CustomerName} on {OrderDate} ({MealType})", order.Id, order.CustomerName, order.OrderDate, order.MealType);

            return MapToOrderDto(order);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new KeyNotFoundException($"Order with id {id} was not found.");
            }

            return MapToOrderDto(order);
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilterRequest filter, CancellationToken cancellationToken = default)
        {
            ValidateFilter(filter);

            var result = await _orderRepository.GetPagedAsync(filter, cancellationToken);
            return new PagedResult<OrderDto>
            {
                Items = result.Items.Select(MapToOrderDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize,
            };
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new KeyNotFoundException($"Order with id {id} was not found.");
            }

            if (!OrderStatusTransitionValidator.IsValidTransition(order.Status, request.NewStatus))
            {
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {request.NewStatus}.");
            }

            order.Status = request.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated order {OrderId} status to {Status}", order.Id, order.Status);

            return MapToOrderDto(order);
        }

        public Task<bool> CanTransitionStatusAsync(OrderStatus from, OrderStatus to, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(OrderStatusTransitionValidator.IsValidTransition(from, to));
        }

        public async Task<bool> IsMenuAvailableAsync(DateTime date, MealType mealType, CancellationToken cancellationToken = default)
        {
            var menus = await _menuRepository.GetPagedAsync(new MenuFilterRequest
            {
                Page = 1,
                PageSize = 1,
                DateFrom = date.Date,
                DateTo = date.Date,
                MealType = mealType,
            }, cancellationToken);

            return menus.Items.Any();
        }

        private async Task<Menu> LoadMenuForOrderAsync(DateTime date, MealType mealType, CancellationToken cancellationToken)
        {
            var menus = await _menuRepository.GetPagedAsync(new MenuFilterRequest
            {
                Page = 1,
                PageSize = 1,
                DateFrom = date.Date,
                DateTo = date.Date,
                MealType = mealType,
            }, cancellationToken);

            var menu = menus.Items.FirstOrDefault();
            if (menu is null)
            {
                throw new InvalidOperationException("Menu not available for this date/type.");
            }

            return menu;
        }

        private static void ValidateCreateOrderRequest(CreateOrderRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new ArgumentException("Customer name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                throw new ArgumentException("Customer phone is required.");
            }

            if (!Regex.IsMatch(request.CustomerPhone.Trim(), @"^\d{7,15}$"))
            {
                throw new ArgumentException("Invalid phone format.");
            }

            if (request.OrderDate.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Cannot create order for past dates.");
            }

            if (request.Items is null || !request.Items.Any())
            {
                throw new ArgumentException("At least one item required.");
            }

            foreach (var item in request.Items)
            {
                if (item.MenuItemId <= 0)
                {
                    throw new ArgumentException("Menu item id must be greater than zero.");
                }

                if (item.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero.");
                }
            }
        }

        private static void ValidateFilter(OrderFilterRequest filter)
        {
            if (filter.DateFrom.HasValue && filter.DateTo.HasValue && filter.DateFrom.Value.Date > filter.DateTo.Value.Date)
            {
                throw new ArgumentException("dateFrom must be earlier than or equal to dateTo.");
            }
        }

        private static OrderDto MapToOrderDto(OrderRequest order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                OrderDate = order.OrderDate,
                MealType = order.MealType.ToString(),
                Status = order.Status.ToString(),
                TotalQuantity = order.TotalQuantity,
                TotalPrice = order.TotalPrice,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.Items
                    .OrderBy(item => item.Id)
                    .Select(item => new OrderItemDto
                    {
                        Id = item.Id,
                        MenuItemId = item.MenuItemId,
                        MenuItemName = item.MenuItemName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal,
                        SpecialRequest = item.SpecialRequest,
                    })
                    .ToList(),
            };
        }
    }
}