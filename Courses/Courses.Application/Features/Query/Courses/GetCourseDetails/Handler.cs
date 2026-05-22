using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using StepLearning.Shared.Abstraction;

namespace Courses.Application.Features.Query.Courses.GetCourseDetails;
internal sealed class Handler(ICoursesRepository coursesRepository, IEnrollmentService enrollmentService) : IRequestHandler<GetCourseDetailsQuery, Result<CourseDetailsDto>>
{

    public async Task<Result<CourseDetailsDto>> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
    {
        var isEnrolled = await enrollmentService.IsEnrolled(request.StudentId, request.CourseId, cancellationToken);

        if (!isEnrolled)
            return Result<CourseDetailsDto>.Failure("Student is not enrolled in this course");

        var courseDetails = await coursesRepository.GetCourseDetailsAsync(request.CourseId, cancellationToken);

        return courseDetails is null
            ? Result<CourseDetailsDto>.Failure("Course not found")
            : Result<CourseDetailsDto>.Success(courseDetails);
    }
}


