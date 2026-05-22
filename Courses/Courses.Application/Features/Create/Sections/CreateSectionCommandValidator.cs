
namespace Courses.Application.Features.Create.Sections;

internal class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.Details.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Details.CourseId)
            .NotEmpty();
    }
}
