using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;
internal sealed class Handler(ICoursesRepository coursesRepository) : IRequestHandler<UpdateCourseCommand, Result>
{

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await coursesRepository.GetCourseByIdEntityAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure("Course not found.");

        if (request.InstructorId != course.InstructorId)
            return Result.Failure("You are not the owner of this course.");

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


