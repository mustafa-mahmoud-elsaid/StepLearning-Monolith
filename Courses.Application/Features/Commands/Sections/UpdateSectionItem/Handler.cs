using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.UpdateSectionItem;

public class Handler : IRequestHandler<UpdateSectionItemCommand, Result>
{
    private readonly ISectionsRepository _sectionsRepository;

    public Handler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }

    public async Task<Result> Handle(UpdateSectionItemCommand request, CancellationToken cancellationToken)
    {
        var sectionItem = await _sectionsRepository.GetSectionItemByIdAsync(request.SectionItemId, cancellationToken);

        if (sectionItem is null)
            return Result.Failure("Section item not found.");

        if (request.Title is not null)
            sectionItem.Title = request.Title;

        await _sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
