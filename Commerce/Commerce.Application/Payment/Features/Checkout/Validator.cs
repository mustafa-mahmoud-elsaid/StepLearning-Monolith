
namespace Commerce.Application.Payment.Features.Checkout;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id must not be empty");
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order id must not be empty");
    }
}
