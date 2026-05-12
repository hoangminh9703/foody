using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Commands
{
    public class LoginCommand : IRequest<AuthResult>
    {
        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Password { get; }
    }
}