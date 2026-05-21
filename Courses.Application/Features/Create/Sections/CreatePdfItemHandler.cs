using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public class CreatePdfItemHandler : IRequestHandler<CreatePdfItemCommand, Result<Guid>>
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

        var pdf = new PdfItem
        {
            Id = Guid.NewGuid(),
            Title = request.Details.Title,
            FileUrl = request.Details.PdfUrl,
            SectionId = request.Details.SectionId
        };

        var lastSectionItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.Id == pdf.SectionId);

        pdf.DisplayOrder = DisplayOrderCalculator.GetNext(lastSectionItemOrder);

        await _sectionsRepository.CreatePdfItemAsync(pdf, cancellationToken);

        return Result<Guid>.Success(pdf.Id);
    }
}
