using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public GetOrderByIdQuery(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}