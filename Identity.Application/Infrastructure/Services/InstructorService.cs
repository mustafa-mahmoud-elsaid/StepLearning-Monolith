using Identity.Application.Domain.Entities;
using Identity.Application.RepositoryInterfaces;
using StepLearning.Shared.Abstraction;

namespace Identity.Application.Infrastructure.Services;

internal sealed class InstructorService(IGenericRepository<Instructor> instructorRepository) : IInstructorService
{
    private readonly IGenericRepository<Instructor> _instructorRepository = instructorRepository;

    public async Task<bool> Exists(Guid instructorId, CancellationToken ct = default)
    {
        if (instructorId == Guid.Empty)
            return false;

        return await _instructorRepository.Exists(x => x.Id == instructorId, ct);
    }
}
