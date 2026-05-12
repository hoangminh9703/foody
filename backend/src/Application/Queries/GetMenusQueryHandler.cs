using MediatR;
using Medicare.Application.DTOs;
using Medicare.Application.Services;

namespace Medicare.Application.Queries
{
    public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, PagedResult<MenuDto>>
    {
        private readonly IMenuService _menuService;

        public GetMenusQueryHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public Task<PagedResult<MenuDto>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
        {
            return _menuService.GetMenusAsync(request.Filter, cancellationToken);
        }
    }
}
