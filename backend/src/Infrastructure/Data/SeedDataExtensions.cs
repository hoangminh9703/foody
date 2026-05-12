using Medicare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Infrastructure.Data
{
    public static class SeedDataExtensions
    {
        public static async Task SeedInitialDataAsync(this MedicareDbContext context)
        {
            var shouldSave = false;

            if (!await context.Users.AnyAsync(user => user.Email == "admin@medicare.local"))
            {
                context.Users.Add(new User
                {
                    Email = "admin@medicare.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Password: Admin123!
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                });

                shouldSave = true;
            }

            var requiredContents = new List<SiteContent>
            {
                new SiteContent
                {
                    Key = "CompanyName",
                    Value = "Medicare",
                    ContentType = "text",
                    Description = "Company name displayed on landing page",
                    CreatedAt = DateTime.UtcNow,
                },
                new SiteContent
                {
                    Key = "CompanyPhone",
                    Value = "+84 123 456 7890",
                    ContentType = "text",
                    Description = "Company contact phone number",
                    CreatedAt = DateTime.UtcNow,
                },
                new SiteContent
                {
                    Key = "CompanyDescription",
                    Value = "Đặt cơm ngon mỗi ngày - Giao hàng nhanh - Menu thay đổi hàng ngày",
                    ContentType = "text",
                    Description = "Company tagline",
                    CreatedAt = DateTime.UtcNow,
                },
            };

            foreach (var content in requiredContents)
            {
                var exists = await context.SiteContents.AnyAsync(existing => existing.Key == content.Key);
                if (exists)
                {
                    continue;
                }

                context.SiteContents.Add(content);
                shouldSave = true;
            }

            if (shouldSave)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
