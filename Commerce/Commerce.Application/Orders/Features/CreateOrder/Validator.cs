using FluentValidation;

namespace Commerce.Application.Orders.Features.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id must not be empty");
    }
}
