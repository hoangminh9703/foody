using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Queries
{
    public class GetOrdersQuery : IRequest<PagedResult<OrderDto>>
    {
        public GetOrdersQuery(OrderFilterRequest filter)
        {
            Filter = filter;
        }

        public OrderFilterRequest Filter { get; }
    }
}