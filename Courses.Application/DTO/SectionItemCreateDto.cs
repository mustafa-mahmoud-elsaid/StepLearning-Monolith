namespace Courses.Application.DTO;

public record VideoCreateDto(string Title, Guid SectionId, TimeSpan Duration, string VideoUrl);
public record PdfCreateDto(string Title, Guid SectionId, string PdfUrl);

