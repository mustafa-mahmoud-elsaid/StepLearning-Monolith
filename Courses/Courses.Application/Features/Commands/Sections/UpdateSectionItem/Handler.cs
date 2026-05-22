using Courses.Application.RepositoriesContracts;
using Courses.Application.RepositoriesContracts;

namespace Courses.Application.Features.Commands.Sections.UpdateSectionItem;
internal sealed class Handler(ISectionsRepository _sectionsRepository) : IRequestHandler<UpdateSectionItemCommand, Result>
{

    public async Task<Result> Handle(UpdateSectionItemCommand request, CancellationToken cancellationToken)
    {
        var sectionItem = await _sectionsRepository.GetSectionItemByIdAsync(request.SectionItemId, cancellationToken);

        if (sectionItem is null)
            return Result.Failure("Section Item not found.");

        if (request.Title is not null)
            sectionItem.UpdateTitle(request.Title);

        await _sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
