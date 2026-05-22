namespace Courses.Application.DTO;

public record CourseDetailsDto(
    Guid Id,
    string Title,
    string? Description,
    string? Thumbnail,
    IEnumerable<SectionDetailsDto> Sections);
