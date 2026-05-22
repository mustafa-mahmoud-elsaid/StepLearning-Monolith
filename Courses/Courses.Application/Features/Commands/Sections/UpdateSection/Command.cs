
namespace Courses.Application.Features.Commands.Sections.UpdateSection;

public record UpdateSectionCommand(Guid SectionId, string? Title) : IRequest<Result>;
