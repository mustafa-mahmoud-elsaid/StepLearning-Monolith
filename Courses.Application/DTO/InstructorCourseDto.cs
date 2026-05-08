namespace Courses.Application.DTO;

public record InstructorCourseDto(
    Guid Id,
    string Title,
    string? Description,
    string? Thumbnail,
    decimal Price,
    bool IsPublished,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastUpdated);
