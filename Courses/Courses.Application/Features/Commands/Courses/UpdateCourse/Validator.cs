using FluentValidation;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;

internal sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");

        RuleFor(x => x.Details.Title)
            .NotEmpty()
            .MaximumLength(250)
            .When(x => x.Details.Title is not null)
            .WithMessage("Title must not be empty and must not exceed 250 characters.");

        RuleFor(x => x.Details.Description)
            .MaximumLength(1000)
            .When(x => x.Details.Description is not null)
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Details.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Details.Price.HasValue)
            .WithMessage("Price must not be negative.");
    }
}
