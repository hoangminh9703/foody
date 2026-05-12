using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            return _orderService.GetOrderByIdAsync(request.OrderId, cancellationToken);
        }
    }
}