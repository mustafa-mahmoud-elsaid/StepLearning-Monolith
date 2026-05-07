namespace Courses.Application.DTO;

// TODO: Add Slug (string) for SEO-friendly URLs.
// TODO: Add AverageRating (double) and ReviewCount (int) once the ratings feature is implemented.
public record CourseCardDto(
    Guid Id,
    string Title,
    string? Description,
    string? Thumbnail,
    decimal Price,
    DateTimeOffset LastUpdated);
