using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetInstructorCourses;

public class Handler : IRequestHandler<GetInstructorCoursesQuery, Result<PaginatedResult<InstructorCourseDto>>>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result<PaginatedResult<InstructorCourseDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
    {
        // TODO: Validate that the InstructorId (request.InstructorId) belongs to a valid instructor.

        var result = await _coursesRepository.GetInstructorCoursesAsync(
            request.InstructorId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<InstructorCourseDto>>.Success(result);
    }
}
