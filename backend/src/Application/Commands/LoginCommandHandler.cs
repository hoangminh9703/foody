using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;

namespace Medicare.Application.Commands
{
    public class LoginCommandHandler
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public Task<AuthResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            return _authenticationService.LoginAsync(command.Email, command.Password, cancellationToken);
        }
    }
}