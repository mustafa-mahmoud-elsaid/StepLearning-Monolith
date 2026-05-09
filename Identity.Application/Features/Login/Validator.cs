using FluentValidation;
using Identity.Application.Domain.DTO;

namespace Identity.Application.Features.Login;

public sealed class Validator : AbstractValidator<LoginCommand>
{
    public Validator()
    {
        RuleFor(x => x.dto.Email)
            .NotEmpty()
            //.MaximumLength(256)
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.dto.Password)
            .NotEmpty();


            // not for login

            //.MinimumLength(8)

            //.Matches("[A-Z]")
            //.WithMessage("Password must contain at least one uppercase letter.")
            //.Matches("[a-z]")
            //.WithMessage("Password must contain at least one lowercase letter.")
            //.Matches("[0-9]")
            //.WithMessage("Password must contain at least one number.");

            //.Matches("[^a-zA-Z0-9]")
            //.WithMessage("Password must contain at least one special character.");
    }
}
