using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Queries
{
    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto>
    {
        private readonly IMenuService _menuService;

        public GetMenuByIdQueryHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
        {
            return _menuService.GetMenuByIdAsync(request.MenuId, cancellationToken);
        }
    }
}
