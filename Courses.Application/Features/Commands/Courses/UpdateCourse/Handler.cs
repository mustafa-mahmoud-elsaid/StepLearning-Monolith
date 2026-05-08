using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;

public class Handler : IRequestHandler<UpdateCourseCommand, Result>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _coursesRepository.GetCourseByIdEntityAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        // TODO: Validate that the requesting user is the course instructor (ownership check).

        try
        {
            course.Update(request.Dto.Title, request.Dto.Description, request.Dto.Price);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _coursesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
