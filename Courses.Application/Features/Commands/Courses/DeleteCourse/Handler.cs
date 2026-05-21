using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.DeleteCourse;
public class Handler(ICoursesRepository coursesRepository) : IRequestHandler<DeleteCourseCommand, Result>
{

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await coursesRepository.GetCourseByIdEntityAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        if (request.InstructorId != course.InstructorId)
            return Result.Failure("You are not the owner of this course.");

        try
        {
            course.SoftDelete();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await coursesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


