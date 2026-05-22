using Courses.Application.DTO;
using StepLearning.Shared.Pagination;

namespace Courses.Application.Features.Query.Courses.GetStudentCourses;

public record GetStudentCoursesQuery(Guid studentId, int pageNumber = 1, int pageSize = 10) : IRequest<Result<PaginatedResult<CourseCardDto>>>;
