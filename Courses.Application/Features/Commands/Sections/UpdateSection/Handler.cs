using Courses.Application.RepositoriesContracts;
using MediatR;
using StepLearning.Shared.Result;

namespace Courses.Application.Features.Commands.Sections.UpdateSection;

public class Handler : IRequestHandler<UpdateSectionCommand, Result>
{
    private readonly ISectionsRepository _sectionsRepository;

    public Handler(ISectionsRepository sectionsRepository)
    {
        _sectionsRepository = sectionsRepository;
    }

    public async Task<Result> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _sectionsRepository.GetSectionByIdAsync(request.SectionId, cancellationToken);

        if (section is null)
            return Result.Failure("Section not found.");

        if (request.Title is not null)
            section.Title = request.Title;

        await _sectionsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
