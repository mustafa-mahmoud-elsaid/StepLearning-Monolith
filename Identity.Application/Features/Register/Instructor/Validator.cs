using FluentValidation;

namespace Identity.Application.Features.Register.Instructor;

public sealed class Validator : AbstractValidator<InstructorRegisterCommand>
{
    public Validator()
    {
        RuleFor(x => x.Credentials.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Credentials.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Credentials.ProfilePictureUrl)
            .MaximumLength(500)
            .Must(uri =>
                string.IsNullOrWhiteSpace(uri) ||
                Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Profile picture URL is invalid.");


        RuleFor(x => x.Credentials.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress()
            .WithMessage("Invalid email format");

        

        RuleFor(x => x.Credentials.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.");

        //.Matches("[^a-zA-Z0-9]")
        //.WithMessage("Password must contain at least one special character.");
    }
}
