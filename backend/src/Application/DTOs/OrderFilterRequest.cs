using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class OrderFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public MealType? MealType { get; set; }
        public string? CustomerName { get; set; }
    }
}