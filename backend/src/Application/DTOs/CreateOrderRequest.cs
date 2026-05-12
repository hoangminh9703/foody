using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public MealType MealType { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
    }
}