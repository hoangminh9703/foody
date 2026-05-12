using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatus NewStatus { get; set; }
    }
}