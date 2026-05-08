using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.DeleteCourse;

public class Handler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _coursesRepository.GetCourseByIdEntityAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        // TODO: Validate that the requesting user is the course instructor (ownership check).

        try
        {
            course.SoftDelete();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _coursesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
