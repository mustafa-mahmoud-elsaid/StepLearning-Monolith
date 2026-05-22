using Courses.Application.DTO;

namespace Courses.Application.Features.Commands.Courses.UpdateCourse;

public record UpdateCourseCommand(Guid CourseId, Guid InstructorId, CourseUpdateDto Details) : IRequest<Result>;
