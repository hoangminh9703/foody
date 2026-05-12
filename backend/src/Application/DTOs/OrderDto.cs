namespace Medicare.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}