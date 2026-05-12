using MediatR;

namespace Medicare.Application.Commands
{
    public class DeleteMenuCommand : IRequest<Unit>
    {
        public DeleteMenuCommand(int menuId)
        {
            MenuId = menuId;
        }

        public int MenuId { get; }
    }
}
