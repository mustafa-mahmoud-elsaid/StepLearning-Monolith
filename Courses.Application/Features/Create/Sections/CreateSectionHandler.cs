using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreateSectionHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        // TODO: check course id


        var lastSectionOrder = await _sectionsRepository.GetLastDisplayOrderAsync<Section>(s => s.Id == request.dto.CourseId)!;

        var section = new Section
        {
            Id = Guid.NewGuid(),
            Title = request.dto.Title,
            CourseId = request.dto.CourseId
        };

        section.DisplayOrder = Helper.GetRightOrder(lastSectionOrder);

        var sectionId = await _sectionsRepository.CreateSectionAsync(section, cancellationToken);

        return Result<Guid>.Success(sectionId);
    }
}
