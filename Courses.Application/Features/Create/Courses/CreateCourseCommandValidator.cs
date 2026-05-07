using FluentValidation;

namespace Courses.Application.Features.Create.Courses;

internal sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.dto.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.dto.Description)
            .MaximumLength(1000);

        RuleFor(x => x.dto.InstructorId)
            .NotEmpty();
    }
}
