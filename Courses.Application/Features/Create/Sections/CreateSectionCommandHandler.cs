using Courses.Application.RepositoriesContracts;
using Courses.Domain.Entities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Create.Sections;

public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, Result<Guid>>
{
    private readonly ISectionsRepository _sectionsRepository;

    public CreateSectionCommandHandler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }
    public async Task<Result<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        // TODO: check course id


        var lastSectionOrder = await _sectionsRepository.GetLastDisplayOrderAsync(request.dto.CourseId)!;

        var section = new Section
        {
            Id = Guid.NewGuid(),
            Title = request.dto.Title,
            CourseId = request.dto.CourseId
        };

        if (lastSectionOrder == -1)
            section.DisplayOrder = 10;
        else
            section.DisplayOrder = ((lastSectionOrder / 10) + 1) * 10; // created sections should be match the pattern: 10, 20, 30, ...

        var sectionId = await _sectionsRepository.CreateSectionAsync(section, cancellationToken);

        return Result<Guid>.Success(sectionId);
    }
}
