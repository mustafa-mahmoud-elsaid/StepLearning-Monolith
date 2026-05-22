
namespace Courses.Application.Features.Commands.Courses.DeleteCourse;

public record DeleteCourseCommand(Guid CourseId, Guid InstructorId) : IRequest<Result>;
