using FluentValidation;

namespace Courses.Application.Features.Commands.Sections.ReorderSection;

internal sealed class ReorderSectionCommandValidator : AbstractValidator<ReorderSectionCommand>
{
    public ReorderSectionCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");

        RuleFor(x => x.Reorder.ItemId)
            .NotEmpty()
            .WithMessage("Item Id must not be empty.");

        RuleFor(x => x.Reorder)
            .Must(r => r.PreviousItemId.HasValue || r.NextItemId.HasValue)
            .WithMessage("At least one of PreviousItemId or NextItemId must be provided.");

        RuleFor(x => x.Reorder)
            .Must(r => r.PreviousItemId != r.ItemId && r.NextItemId != r.ItemId)
            .WithMessage("Item cannot be its own neighbor.");
    }
}
