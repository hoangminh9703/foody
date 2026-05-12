using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Interfaces;

namespace Medicare.Application.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return _authenticationService.LoginAsync(request.Email, request.Password, cancellationToken);
        }
    }
}