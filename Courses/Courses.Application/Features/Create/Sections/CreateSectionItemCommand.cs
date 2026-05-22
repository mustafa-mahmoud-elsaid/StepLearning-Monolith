using Courses.Application.DTO;

namespace Courses.Application.Features.Create.Sections;

public record CreateVideoItemCommand(VideoCreateDto Details): IRequest<Result<Guid>>;
public record CreatePdfItemCommand(PdfCreateDto Details): IRequest<Result<Guid>>;
