using MediatR;
using Medicare.Application.DTOs;

namespace Medicare.Application.Commands
{
    public class CreateMenuCommand : IRequest<MenuDto>
    {
        public CreateMenuCommand(CreateMenuRequest request)
        {
            Request = request;
        }

        public CreateMenuRequest Request { get; }
    }
}
