namespace Medicare.Application.DTOs
{
    public class MenuDto
    {
        public int Id { get; set; }
        public DateTime DateApplied { get; set; }
        public string MealType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MenuItemDto> Items { get; set; } = new();
    }
}
