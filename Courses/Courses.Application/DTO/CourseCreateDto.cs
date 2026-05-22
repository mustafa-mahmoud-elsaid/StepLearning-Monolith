namespace Courses.Application.DTO;

public record CourseCreateDto(Guid InstructorId, string Title, string? Description);
