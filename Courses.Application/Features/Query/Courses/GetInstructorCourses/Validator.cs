using FluentValidation;

namespace Courses.Application.Features.Query.Courses.GetInstructorCourses;

internal sealed class GetInstructorCoursesQueryValidator : AbstractValidator<GetInstructorCoursesQuery>
{
    public GetInstructorCoursesQueryValidator()
    {
        RuleFor(x => x.InstructorId)
            .NotEmpty()
            .WithMessage("Instructor Id must not be empty.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");
    }
}
