namespace Courses.Application.DTO;

public record SectionItemDetailsDto(
    Guid Id,
    string Title,
    string ItemType,
    string ContentUrl,
    TimeSpan? Duration);
