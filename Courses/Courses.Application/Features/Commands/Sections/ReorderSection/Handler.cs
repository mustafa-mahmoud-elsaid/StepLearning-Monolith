using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;

namespace Courses.Application.Features.Commands.Sections.ReorderSection;
internal sealed class Handler(ISectionsRepository sectionsRepository) : IRequestHandler<ReorderSectionCommand, Result>
{

    public async Task<Result> Handle(ReorderSectionCommand request, CancellationToken cancellationToken)
    {
        var sections = await sectionsRepository.GetSectionsByCourseIdAsync(request.CourseId, cancellationToken);

        if (sections.Count == 0)
            return Result.Failure("No sections found for this course.");

        try
        {
            ReorderService.Reorder(
                sections,
                request.Reorder.ItemId,
                request.Reorder.PreviousItemId,
                request.Reorder.NextItemId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


