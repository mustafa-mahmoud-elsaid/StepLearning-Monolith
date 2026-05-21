using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Pagination;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Query.Courses.GetInstructorCourses;

public record GetInstructorCoursesQuery(Guid InstructorId, int PageNumber = 1, int PageSize = 10)
    : IRequest<Result<PaginatedResult<InstructorCourseDto>>>;
