namespace Courses.Application.DTO;

public record CoursePreviewDto(Guid Id, string Title, string? Description, string? Thumbnail, decimal? Price, IEnumerable<SectionPreviewDto> Sections);