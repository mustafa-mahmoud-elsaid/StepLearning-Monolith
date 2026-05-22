
namespace Enrollment.Application.Features.Commands.Enroll;

internal class Validator : AbstractValidator<EnrollStudentCommand>
{
    public Validator()
    {
        RuleFor(e => e.Details.CourseIds)
            .NotEmpty()
            .WithMessage("Course Ids must not be empty");

        RuleFor(e => e.Details.StudentId)
            .NotEmpty()
            .WithMessage("Student Id must not be empty");

        RuleFor(e => e.Details.PaymentId)
            .NotEmpty()
            .WithMessage("Payment Id must not be empty");

        RuleFor(e => e.Details.Status)
            .NotNull()
            .IsInEnum();
    }
}
