using MediatR;

namespace Medicare.Application.Queries
{
    public class GetMenuByIdQuery : IRequest<Medicare.Application.DTOs.MenuDto>
    {
        public GetMenuByIdQuery(int menuId)
        {
            MenuId = menuId;
        }

        public int MenuId { get; }
    }
}
