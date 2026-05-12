using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Commands
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
    {
        private readonly IMenuService _menuService;

        public CreateMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            return _menuService.CreateMenuAsync(request.Request, cancellationToken);
        }
    }
}
