using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Queries
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderService _orderService;

        public GetOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            return _orderService.GetOrdersAsync(request.Filter, cancellationToken);
        }
    }
}