using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using StepLearning.Shared.Pagination;

namespace Courses.Application.Features.Query.Courses.GetCourseCards;
internal sealed class Handler(ICoursesRepository coursesRepository) : IRequestHandler<GetCourseCardsQuery, Result<PaginatedResult<CourseCardDto>>>
{

    public async Task<Result<PaginatedResult<CourseCardDto>>> Handle(GetCourseCardsQuery request, CancellationToken cancellationToken)
    {
        var result = await coursesRepository.GetCourseCardsAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<CourseCardDto>>.Success(result);
    }
}


