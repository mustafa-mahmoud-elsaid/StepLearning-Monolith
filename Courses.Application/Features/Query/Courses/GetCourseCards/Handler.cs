using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCourseCards;

public class Handler : IRequestHandler<GetCourseCardsQuery, Result<PaginatedResult<CourseCardDto>>>
{
    private readonly ICoursesRepository _coursesRepository;

    public Handler(ICoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public async Task<Result<PaginatedResult<CourseCardDto>>> Handle(GetCourseCardsQuery request, CancellationToken cancellationToken)
    {
        var result = await _coursesRepository.GetCourseCardsAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<CourseCardDto>>.Success(result);
    }
}
