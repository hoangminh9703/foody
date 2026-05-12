namespace Medicare.Domain.Entities
{
    public class SiteContent : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ContentType { get; set; } = "text"; // text, html, json
        public string? Description { get; set; }
    }
}
