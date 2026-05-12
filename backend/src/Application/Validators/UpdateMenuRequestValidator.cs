using FluentValidation;
using Medicare.Application.DTOs;

namespace Medicare.Application.Validators
{
    public class UpdateMenuRequestValidator : AbstractValidator<UpdateMenuRequest>
    {
        public UpdateMenuRequestValidator()
        {
            RuleFor(x => x.DateApplied).NotEmpty().WithMessage("DateApplied is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleForEach(x => x.Items).SetValidator(new MenuItemRequestValidator());
        }
    }
}
