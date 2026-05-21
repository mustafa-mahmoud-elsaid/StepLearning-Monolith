using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;
public class Handler(ICoursesRepository coursesRepository) : IRequestHandler<UpdateCourseCommand, Result>
{

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await coursesRepository.GetCourseByIdEntityAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        // TODO: Validate that the requesting user is the course instructor (ownership check).

        try
        {
            course.Update(request.Details.Title, request.Details.Description, request.Details.Price);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }

        await coursesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


