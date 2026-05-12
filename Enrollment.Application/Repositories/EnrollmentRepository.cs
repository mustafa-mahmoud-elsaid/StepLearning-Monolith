
using Enrollment.Application.Data;

namespace Enrollment.Application.Repositories;

internal class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentDbContext _dbContext;

    public EnrollmentRepository(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddEnrollment(Domain.Entities.Enrollment enrollment, CancellationToken ct = default)
    {
        await _dbContext.Enrollments.AddAsync(enrollment, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
