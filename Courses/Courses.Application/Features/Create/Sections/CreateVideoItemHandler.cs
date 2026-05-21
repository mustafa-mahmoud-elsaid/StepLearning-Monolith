using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

internal sealed class CreateVideoItemHandler : IRequestHandler<CreateVideoItemCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreateVideoItemHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreateVideoItemCommand request, CancellationToken cancellationToken)
    {
        var section = await _sectionsRepository.GetSectionByIdAsync(request.Details.SectionId, cancellationToken);
        if (section is null)
            return Result<Guid>.Failure("Section not found.");

        var lastItemOrder = await _sectionsRepository.GetLastDisplayOrderAsync<SectionItem>(s => s.SectionId == request.Details.SectionId);
        var nextOrder = DisplayOrderCalculator.GetNext(lastItemOrder);

        var video = VideoItem.Create(request.Details.Title, request.Details.VideoUrl, request.Details.Duration, request.Details.SectionId, nextOrder);

        await _sectionsRepository.CreateVideoItemAsync(video, cancellationToken);

        return Result<Guid>.Success(video.Id);
    }
}
