namespace Medicare.Application.DTOs
{
    public class OrderItemRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialRequest { get; set; }
    }
}