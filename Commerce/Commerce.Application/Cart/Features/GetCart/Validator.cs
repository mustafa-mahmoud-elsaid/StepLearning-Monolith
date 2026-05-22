
namespace Commerce.Application.Cart.Features.GetCart;

public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student id must not be empty");
    }
}
