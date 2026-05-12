using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Commands
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public CreateOrderCommand(CreateOrderRequest request)
        {
            Request = request;
        }

        public CreateOrderRequest Request { get; }
    }
}