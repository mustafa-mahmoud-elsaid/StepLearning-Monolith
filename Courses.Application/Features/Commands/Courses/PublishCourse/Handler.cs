using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.PublishCourse;

public class Handler : IRequestHandler<PublishCourseCommand, Result>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _coursesRepository.GetCourseWithSectionsAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        // TODO: Validate that request.InstructorId matches course.InstructorId (ownership check).

        try
        {
            course.Publish();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _coursesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
