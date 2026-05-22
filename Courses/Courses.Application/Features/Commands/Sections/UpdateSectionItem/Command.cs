
namespace Courses.Application.Features.Commands.Sections.UpdateSectionItem;

public record UpdateSectionItemCommand(Guid SectionItemId, string? Title) : IRequest<Result>;
