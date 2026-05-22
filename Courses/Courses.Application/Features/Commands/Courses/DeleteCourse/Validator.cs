
namespace Courses.Application.Features.Commands.Courses.DeleteCourse;

internal sealed class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");
    }
}
