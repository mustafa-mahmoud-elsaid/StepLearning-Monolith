
namespace Courses.Application.Features.Commands.Courses.PublishCourse;

public record PublishCourseCommand(Guid CourseId, Guid InstructorId) : IRequest<Result>;
