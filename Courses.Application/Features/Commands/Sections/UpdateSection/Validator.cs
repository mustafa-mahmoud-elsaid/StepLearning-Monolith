using FluentValidation;

namespace Courses.Application.Features.Commands.Sections.UpdateSection;

internal sealed class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage("Section Id must not be empty.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250)
            .When(x => x.Title is not null)
            .WithMessage("Title must not be empty and must not exceed 250 characters.");
    }
}
