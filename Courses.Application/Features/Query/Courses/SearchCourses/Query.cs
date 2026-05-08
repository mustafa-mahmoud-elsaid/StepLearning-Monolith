using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.SearchCourses;

// TODO: Add Slug filter once slug property is added to the Course entity.
public record SearchCoursesQuery(
    string? Title = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PaginatedResult<CourseCardDto>>>;
