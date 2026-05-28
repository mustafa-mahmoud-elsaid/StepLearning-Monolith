
namespace Commerce.Application.Cart.Features.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course id must not be empty");
    }
}
