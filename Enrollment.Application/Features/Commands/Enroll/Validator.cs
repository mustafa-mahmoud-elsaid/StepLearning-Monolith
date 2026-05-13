using FluentValidation;

namespace Enrollment.Application.Features.Commands.Enroll;

internal class Validator : AbstractValidator<EnrollStudentCommand>
{
    public Validator()
    {
        RuleFor(e => e.dto.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty");

        RuleFor(e => e.dto.StudentId)
            .NotEmpty()
            .WithMessage("Student Id must not be empty");

        RuleFor(e => e.dto.Status)
            .NotNull()
            .IsInEnum();
    }
}
