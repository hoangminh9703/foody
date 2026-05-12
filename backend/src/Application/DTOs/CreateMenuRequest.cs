using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class CreateMenuRequest
    {
        public DateTime DateApplied { get; set; }
        public MealType MealType { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<MenuItemRequest> Items { get; set; } = new();
    }
}
