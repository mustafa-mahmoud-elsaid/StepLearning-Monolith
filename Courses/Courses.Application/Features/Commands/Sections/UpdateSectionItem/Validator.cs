
namespace Courses.Application.Features.Commands.Sections.UpdateSectionItem;

internal sealed class UpdateSectionItemCommandValidator : AbstractValidator<UpdateSectionItemCommand>
{
    public UpdateSectionItemCommandValidator()
    {
        RuleFor(x => x.SectionItemId)
            .NotEmpty()
            .WithMessage("Section item Id must not be empty.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250)
            .When(x => x.Title is not null)
            .WithMessage("Title must not be empty and must not exceed 250 characters.");
    }
}
