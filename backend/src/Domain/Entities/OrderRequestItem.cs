namespace Medicare.Domain.Entities
{
    public class OrderRequestItem : BaseEntity
    {
        public int OrderRequestId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? SpecialRequest { get; set; }

        // Navigation properties
        public OrderRequest? OrderRequest { get; set; }
    }
}
