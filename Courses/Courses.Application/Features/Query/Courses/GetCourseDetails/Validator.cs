using FluentValidation;

namespace Courses.Application.Features.Query.Courses.GetCourseDetails;

internal sealed class GetCourseDetailsQueryValidator : AbstractValidator<GetCourseDetailsQuery>
{
    public GetCourseDetailsQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");

        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student Id must not be empty.");
    }
}
