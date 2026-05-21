using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public class CreateVideoItemHandler : IRequestHandler<CreateVideoItemCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreateVideoItemHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreateVideoItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: Check section id


        var video = new VideoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Details.Title,
            VideoUrl = request.Details.VideoUrl,
            Duration = request.Details.Duration, // for now
            SectionId = request.Details.SectionId
        };

        var lastItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.Id == video.SectionId);

        video.DisplayOrder = DisplayOrderCalculator.GetNext(lastItemOrder);

        await _sectionsRepository.CreateVideoItemAsync(video, cancellationToken);

        return Result<Guid>.Success(video.Id);
    }
}
public class CreatePdfItemHandler : IRequestHandler<CreatePdfItemCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreatePdfItemHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreatePdfItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: Check section id

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
