using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.UpdateSection;
internal sealed class Handler(ISectionsRepository sectionsRepository) : IRequestHandler<UpdateSectionCommand, Result>
{

    public async Task<Result> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await sectionsRepository.GetSectionByIdAsync(request.SectionId, cancellationToken);

        if (section is null)
            return Result.Failure("Section not found.");

        if (request.Title is not null)
            section.Title = request.Title;

        await sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


