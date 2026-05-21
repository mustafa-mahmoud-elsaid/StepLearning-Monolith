using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.UpdateSectionItem;
internal sealed class Handler(ISectionsRepository sectionsRepository) : IRequestHandler<UpdateSectionItemCommand, Result>
{

    public async Task<Result> Handle(UpdateSectionItemCommand request, CancellationToken cancellationToken)
    {
        var sectionItem = await sectionsRepository.GetSectionItemByIdAsync(request.SectionItemId, cancellationToken);

        if (sectionItem is null)
            return Result.Failure("Section item not found.");

        if (request.Title is not null)
            sectionItem.Title = request.Title;

        await sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


