
namespace Courses.Application.Features.Create.Courses;

internal sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Details.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Details.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Details.InstructorId)
            .NotEmpty();
    }
}
