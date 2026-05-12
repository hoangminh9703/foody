using Microsoft.AspNetCore.Http;

namespace Medicare.Api.Middleware
{
    public class SessionValidationMiddleware
    {
        private static readonly PathString HealthPath = new("/api/health");
        private static readonly PathString LoginPath = new("/api/auth/login");
        private static readonly PathString LogoutPath = new("/api/auth/logout");
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments(HealthPath, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments(LoginPath, StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments(LogoutPath, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                _logger.LogWarning("Unauthorized API access attempt to {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Unauthorized",
                    timestamp = DateTime.UtcNow,
                });
                return;
            }

            context.Items["AuthenticatedUserId"] = userId.Value;
            context.Items["AuthenticatedEmail"] = context.Session.GetString("Email");
            context.Items["AuthenticatedFullName"] = context.Session.GetString("FullName");
            context.Items["AuthenticatedRole"] = context.Session.GetString("Role");

            await _next(context);
        }
    }
}