using Courses.Application.DTO;
using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.SearchCourses;
public class Handler(ICoursesRepository coursesRepository) : IRequestHandler<SearchCoursesQuery, Result<PaginatedResult<CourseCardDto>>>
{

    public async Task<Result<PaginatedResult<CourseCardDto>>> Handle(SearchCoursesQuery request, CancellationToken cancellationToken)
    {
        var result = await coursesRepository.SearchCoursesAsync(
            request.Title,
            request.MinPrice,
            request.MaxPrice,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PaginatedResult<CourseCardDto>>.Success(result);
    }
}


