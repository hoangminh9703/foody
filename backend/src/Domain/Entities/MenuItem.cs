namespace Medicare.Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        public int MenuId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation property
        public Menu? Menu { get; set; }
    }
}
