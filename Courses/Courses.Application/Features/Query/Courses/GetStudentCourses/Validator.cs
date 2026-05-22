
namespace Courses.Application.Features.Query.Courses.GetStudentCourses;

internal sealed class Validator : AbstractValidator<GetStudentCoursesQuery>
{
    public Validator()
    {
        RuleFor(x => x.studentId).NotEmpty().WithMessage("StudentId is required.");
        RuleFor(x => x.pageNumber).GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");
        RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.");
    }
}
