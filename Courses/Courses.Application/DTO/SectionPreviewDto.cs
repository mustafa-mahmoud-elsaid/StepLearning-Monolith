namespace Courses.Application.DTO;

public record SectionPreviewDto(Guid Id, string Title, IEnumerable<SectionItemPreviewDto> SectionItems);