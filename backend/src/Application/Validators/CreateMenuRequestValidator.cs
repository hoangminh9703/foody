using FluentValidation;
using Medicare.Application.DTOs;

namespace Medicare.Application.Validators
{
    public class MenuItemRequestValidator : AbstractValidator<MenuItemRequest>
    {
        public MenuItemRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Item name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }

    public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
    {
        public CreateMenuRequestValidator()
        {
            RuleFor(x => x.DateApplied).NotEmpty().WithMessage("DateApplied is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleForEach(x => x.Items).SetValidator(new MenuItemRequestValidator());
        }
    }
}
