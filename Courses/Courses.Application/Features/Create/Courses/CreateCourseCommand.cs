using Courses.Application.DTO;

namespace Courses.Application.Features.Create.Courses;

public record CreateCourseCommand(CourseCreateDto Details) : IRequest<Result<Guid>>;

