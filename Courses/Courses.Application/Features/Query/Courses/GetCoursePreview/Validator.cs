using FluentValidation;

namespace Courses.Application.Features.Query.Courses.GetCoursePreview;

internal sealed class GetCoursePreviewQueryValidator : AbstractValidator<GetCoursePreviewQuery>
{
    public GetCoursePreviewQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");
    }
}
