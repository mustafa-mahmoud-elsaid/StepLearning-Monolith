using FluentValidation;

namespace Courses.Application.Features.Create.Sections;

internal class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.dto.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.dto.CourseId)
            .NotEmpty();
    }
}
