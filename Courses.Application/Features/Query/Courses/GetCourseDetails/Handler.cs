using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCourseDetails;

public class Handler : IRequestHandler<GetCourseDetailsQuery, Result<CourseDetailsDto>>
{
    private readonly ICoursesRepository _coursesRepository;
    private readonly IEnrollmentService _enrollmentService;

    public Handler(ICoursesRepository coursesRepository, IEnrollmentService enrollmentService)
    {
        _coursesRepository = coursesRepository;
        _enrollmentService = enrollmentService;
    }

    public async Task<Result<CourseDetailsDto>> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Validate that the student (request.StudentId) is enrolled in the course before returning details.

        var isEnrolled = await _enrollmentService.IsEnrolled(request.StudentId, request.CourseId, cancellationToken);

        if (!isEnrolled)
            return Result<CourseDetailsDto>.Failure("Student is not enrolled in this course");

        var courseDetails = await _coursesRepository.GetCourseDetailsAsync(request.CourseId, cancellationToken);

        return courseDetails is null
            ? Result<CourseDetailsDto>.Failure("Course not found")
            : Result<CourseDetailsDto>.Success(courseDetails);
    }
}
