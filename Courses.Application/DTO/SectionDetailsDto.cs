namespace Courses.Application.DTO;

public record SectionDetailsDto(
    Guid Id,
    string Title,
    IEnumerable<SectionItemDetailsDto> SectionItems);
