using Medicare.Api.Contracts;
using Medicare.Application.Commands;
using Medicare.Application.DTOs;
using Medicare.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Medicare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string SessionUserIdKey = "UserId";
        private const string SessionEmailKey = "Email";
        private const string SessionFullNameKey = "FullName";
        private const string SessionRoleKey = "Role";
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticate an admin user and create a session.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            SetSession(result);
            return Ok(result);
        }

        /// <summary>
        /// Return the currently authenticated user from session.
        /// </summary>
        [HttpGet("me")]
        public ActionResult<AuthResult> Me()
        {
            if (!TryBuildSessionUser(out var currentUser))
            {
                return Unauthorized(new AuthResult
                {
                    Success = false,
                    Message = "Unauthorized",
                    ExpiresAt = DateTime.UtcNow,
                });
            }

            return Ok(currentUser);
        }

        /// <summary>
        /// Clear the current auth session.
        /// </summary>
        [HttpPost("logout")]
        public ActionResult<AuthResult> Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Logged out successfully",
                ExpiresAt = DateTime.UtcNow,
            });
        }

        private void SetSession(AuthResult authResult)
        {
            HttpContext.Session.SetInt32(SessionUserIdKey, authResult.UserId);
            HttpContext.Session.SetString(SessionEmailKey, authResult.Email);
            HttpContext.Session.SetString(SessionFullNameKey, authResult.FullName);
            HttpContext.Session.SetString(SessionRoleKey, authResult.Role.ToString());
        }

        private bool TryBuildSessionUser(out AuthResult authResult)
        {
            authResult = new AuthResult();

            var userId = HttpContext.Session.GetInt32(SessionUserIdKey);
            var email = HttpContext.Session.GetString(SessionEmailKey);
            var fullName = HttpContext.Session.GetString(SessionFullNameKey);
            var role = HttpContext.Session.GetString(SessionRoleKey);

            if (!userId.HasValue || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            if (!Enum.TryParse(role, true, out UserRole parsedRole))
            {
                parsedRole = UserRole.Admin;
            }

            authResult = new AuthResult
            {
                Success = true,
                Message = "Authenticated",
                UserId = userId.Value,
                Email = email,
                FullName = fullName,
                Role = parsedRole,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            };

            return true;
        }
    }
}