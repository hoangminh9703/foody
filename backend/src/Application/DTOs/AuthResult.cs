using Medicare.Domain.Entities;

namespace Medicare.Application.DTOs
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}