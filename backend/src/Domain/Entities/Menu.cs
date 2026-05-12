namespace Medicare.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public DateTime DateApplied { get; set; }
        public MealType MealType { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    public enum MealType
    {
        Lunch = 1,
        Dinner = 2,
    }
}
