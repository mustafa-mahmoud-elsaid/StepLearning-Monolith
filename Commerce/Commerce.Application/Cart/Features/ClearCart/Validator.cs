
namespace Commerce.Application.Cart.Features.ClearCart;

public class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id must not be empty");
    }
}
