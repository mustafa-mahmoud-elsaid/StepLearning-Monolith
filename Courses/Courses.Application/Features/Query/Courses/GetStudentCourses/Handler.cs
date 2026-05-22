using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Pagination;

namespace Courses.Application.Features.Query.Courses.GetStudentCourses;

internal sealed class Handler(IEnrollmentService enrollmentService, ICoursesRepository coursesRepository)
    : IRequestHandler<GetStudentCoursesQuery, Result<PaginatedResult<CourseCardDto>>>
{
    public async Task<Result<PaginatedResult<CourseCardDto>>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
    {
        var coursesIds = (await enrollmentService
            .GetStudentCourses(request.studentId, cancellationToken))
            .ToList();

        if (coursesIds.Count == 0)
            return Result<PaginatedResult<CourseCardDto>>.Success(new PaginatedResult<CourseCardDto>
            {
                TotalCount = 0,
                Data = [],
                PageNumber = request.pageNumber,
                PageSize = request.pageSize
            });

        var result = await coursesRepository.GetStudentCoursesAsync(
            coursesIds,
            request.pageNumber,
            request.pageSize,
            cancellationToken);

        return Result<PaginatedResult<CourseCardDto>>.Success(result);
    }
}


