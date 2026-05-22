
namespace Commerce.Application.Cart.Features.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id must not be empty");
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course id must not be empty");
    }
}
