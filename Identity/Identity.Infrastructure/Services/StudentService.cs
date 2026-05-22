using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Identity.Application.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using StepLearning.Shared.Abstraction;

namespace Identity.Infrastructure.Services;

internal class StudentService(
    IGenericRepository<Student> studentRepository,
    UsersDbContext dbContext) : IStudentService
{
    public async Task<bool> Exists(Guid studentId, CancellationToken ct = default)
    {
        if (studentId == Guid.Empty)
            return false;

        return await studentRepository.Exists(s => s.Id == studentId, ct);
    }

    public async Task<string?> GetEmail(Guid studentId, CancellationToken ct = default)
    {
        if (studentId == Guid.Empty)
            return null;

        return await dbContext.Students
            .AsNoTracking()
            .Where(student => student.Id == studentId)
            .Join(
                dbContext.Users.AsNoTracking(),
                student => student.UserId,
                user => user.Id,
                (_, user) => user.Email)
            .FirstOrDefaultAsync(ct);
    }
}
