
namespace Courses.Application.Features.Commands.Courses.PublishCourse;

internal sealed class PublishCourseCommandValidator : AbstractValidator<PublishCourseCommand>
{
    public PublishCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course Id must not be empty.");

        RuleFor(x => x.InstructorId)
            .NotEmpty()
            .WithMessage("Instructor Id must not be empty.");
    }
}
