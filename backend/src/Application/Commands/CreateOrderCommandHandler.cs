using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            return _orderService.CreateOrderAsync(request.Request, cancellationToken);
        }
    }
}