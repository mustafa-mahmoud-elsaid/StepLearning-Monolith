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
            Title = request.dto.Title,
            VideoUrl = request.dto.VideoUrl,
            Duration = request.dto.Duration, // for now
            SectionId = request.dto.SectionId
        };

        var lastItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.Id == video.SectionId);

        video.DisplayOrder = Helper.GetRightOrder(lastItemOrder);

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
            Title = request.dto.Title,
            FileUrl = request.dto.PdfUrl,
            SectionId = request.dto.SectionId
        };

        var lastSectionItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.Id == pdf.SectionId);

        pdf.DisplayOrder = Helper.GetRightOrder(lastSectionItemOrder);

        await _sectionsRepository.CreatePdfItemAsync(pdf, cancellationToken);

        return Result<Guid>.Success(pdf.Id);
    }
}
