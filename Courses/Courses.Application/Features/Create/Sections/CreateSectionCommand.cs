using Courses.Application.DTO;

namespace Courses.Application.Features.Create.Sections;

public record CreateSectionCommand(SectionCreateDto Details) : IRequest<Result<Guid>>;
