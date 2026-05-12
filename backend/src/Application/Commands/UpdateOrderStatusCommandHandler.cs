using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Commands
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IOrderService _orderService;

        public UpdateOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            return _orderService.UpdateOrderStatusAsync(request.OrderId, request.Request, cancellationToken);
        }
    }
}