using MediatR;
using Medicare.Application.Services;

namespace Medicare.Application.Commands
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, Unit>
    {
        private readonly IMenuService _menuService;

        public DeleteMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<Unit> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            await _menuService.DeleteMenuAsync(request.MenuId, cancellationToken);
            return Unit.Value;
        }
    }
}
