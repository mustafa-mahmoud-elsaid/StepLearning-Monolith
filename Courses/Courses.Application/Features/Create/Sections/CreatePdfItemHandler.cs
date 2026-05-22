using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;

namespace Courses.Application.Features.Create.Sections;

internal sealed class CreatePdfItemHandler : IRequestHandler<CreatePdfItemCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreatePdfItemHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreatePdfItemCommand request, CancellationToken cancellationToken)
    {
        var section = await _sectionsRepository.GetSectionByIdAsync(request.Details.SectionId, cancellationToken);
        if (section is null)
            return Result<Guid>.Failure("Section not found.");

        var lastSectionItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.SectionId == request.Details.SectionId);
        var nextOrder = DisplayOrderCalculator.GetNext(lastSectionItemOrder);

        var pdf = PdfItem.Create(request.Details.Title, request.Details.PdfUrl, request.Details.SectionId, nextOrder);

        await _sectionsRepository.CreatePdfItemAsync(pdf, cancellationToken);

        return Result<Guid>.Success(pdf.Id);
    }
}
