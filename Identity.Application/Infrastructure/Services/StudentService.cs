using Identity.Application.Domain.Entities;
using Identity.Application.RepositoryInterfaces;
using StepLearning.Shared.Abstraction;

namespace Identity.Application.Infrastructure.Services;

internal class StudentService(IGenericRepository<Student> studentRepository) : IStudentService
{
    public async Task<bool> Exists(Guid studentId, CancellationToken ct = default)
    {
        if (studentId == Guid.Empty)
            return false;

        return await studentRepository.Exists(s => s.Id == studentId, ct);
    }
}
