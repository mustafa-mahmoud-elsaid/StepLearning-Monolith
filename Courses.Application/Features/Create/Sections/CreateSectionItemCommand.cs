using Courses.Application.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public record CreateVideoItemCommand(VideoCreateDto dto): IRequest<Result<Guid>>;
public record CreatePdfItemCommand(PdfCreateDto dto): IRequest<Result<Guid>>;