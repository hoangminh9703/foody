namespace Medicare.Domain.Entities
{
    public class OrderRequest : BaseEntity
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public MealType MealType { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string Notes { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation property
        public ICollection<OrderRequestItem> Items { get; set; } = new List<OrderRequestItem>();
    }

    public enum OrderStatus
    {
        New = 1,
        Contacted = 2,
        Confirmed = 3,
        Completed = 4,
        Cancelled = 5,
    }
}
