using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class MenuFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public MealType? MealType { get; set; }
    }
}
