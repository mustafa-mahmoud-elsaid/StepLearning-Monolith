using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.ReorderSectionItem;

public class Handler : IRequestHandler<ReorderSectionItemCommand, Result>
{
    private readonly ISectionsRepository _sectionsRepository;

    public Handler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }

    public async Task<Result> Handle(ReorderSectionItemCommand request, CancellationToken cancellationToken)
    {
        var sectionItems = await _sectionsRepository.GetSectionItemsBySectionIdAsync(request.SectionId, cancellationToken);

        if (sectionItems.Count == 0)
            return Result.Failure("No items found for this section.");

        try
        {
            ReorderService.Reorder(
                sectionItems,
                request.Reorder.ItemId,
                request.Reorder.PreviousItemId,
                request.Reorder.NextItemId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
