using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Commands
{
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
    {
        private readonly IMenuService _menuService;

        public UpdateMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            return _menuService.UpdateMenuAsync(request.MenuId, request.Request, cancellationToken);
        }
    }
}
