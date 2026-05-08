using Courses.Application.RepositoriesContracts;
using Courses.Application.Utilities;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.ReorderSection;

public class Handler : IRequestHandler<ReorderSectionCommand, Result>
{
    private readonly ISectionsRepository _sectionsRepository;

    public Handler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }

    public async Task<Result> Handle(ReorderSectionCommand request, CancellationToken cancellationToken)
    {
        var sections = await _sectionsRepository.GetSectionsByCourseIdAsync(request.CourseId, cancellationToken);

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

        await _sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
