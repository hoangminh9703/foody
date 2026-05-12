using FluentValidation;
using Medicare.Application.DTOs;

namespace Medicare.Application.Validators
{
    public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
    {
        public OrderItemRequestValidator()
        {
            RuleFor(x => x.MenuItemId).GreaterThan(0).WithMessage("Menu item id is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            RuleFor(x => x.SpecialRequest).MaximumLength(500).WithMessage("Special request is too long.");
        }
    }

    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(100).WithMessage("Name too long");

            RuleFor(x => x.CustomerPhone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\d{7,15}$").WithMessage("Invalid phone format");

            RuleFor(x => x.OrderDate)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Order date cannot be in the past.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item required");

            RuleForEach(x => x.Items).SetValidator(new OrderItemRequestValidator());
        }
    }

    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.NewStatus).IsInEnum().WithMessage("New status is required.");
        }
    }
}