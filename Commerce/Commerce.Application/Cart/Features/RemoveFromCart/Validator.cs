
namespace Commerce.Application.Cart.Features.RemoveFromCart;

public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course id must not be empty");
    }
}
