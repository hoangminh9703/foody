namespace Medicare.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Admin;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
    }
}
