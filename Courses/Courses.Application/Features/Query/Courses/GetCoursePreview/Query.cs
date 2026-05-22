using Courses.Application.DTO;

namespace Courses.Application.Features.Query.Courses.GetCoursePreview;

public record GetCoursePreviewQuery(Guid Id) : IRequest<Result<CoursePreviewDto>>;
