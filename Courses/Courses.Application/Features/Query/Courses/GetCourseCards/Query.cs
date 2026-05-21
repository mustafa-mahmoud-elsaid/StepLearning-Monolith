using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetCourseCards;

// TODO: Add optional StudentId parameter to filter/rank courses based on user profile preferences.
// TODO: Add optional sorting support (e.g., by rating, popularity, newest).
// TODO: Add slug-based lookup as an alternative to Guid-based course identification.
public record GetCourseCardsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<CourseCardDto>>>;
