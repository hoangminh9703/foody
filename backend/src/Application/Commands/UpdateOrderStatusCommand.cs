using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Commands
{
    public class UpdateOrderStatusCommand : IRequest<OrderDto>
    {
        public UpdateOrderStatusCommand(int orderId, UpdateOrderStatusRequest request)
        {
            OrderId = orderId;
            Request = request;
        }

        public int OrderId { get; }
        public UpdateOrderStatusRequest Request { get; }
    }
}