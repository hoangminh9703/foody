using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Queries
{
    public class GetMenusQuery : IRequest<PagedResult<MenuDto>>
    {
        public GetMenusQuery(MenuFilterRequest filter)
        {
            Filter = filter;
        }

        public MenuFilterRequest Filter { get; }
    }
}
