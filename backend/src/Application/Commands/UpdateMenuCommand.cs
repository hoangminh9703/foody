using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Commands
{
    public class UpdateMenuCommand : IRequest<MenuDto>
    {
        public UpdateMenuCommand(int menuId, UpdateMenuRequest request)
        {
            MenuId = menuId;
            Request = request;
        }

        public int MenuId { get; }
        public UpdateMenuRequest Request { get; }
    }
}
