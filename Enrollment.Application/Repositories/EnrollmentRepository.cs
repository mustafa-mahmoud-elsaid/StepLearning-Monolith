
using Enrollment.Application.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> IsEnrolled(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        return await _dbContext.Enrollments
            .AnyAsync(e => e.CourseId == courseId &&
            e.StudentId == studentId &&
            (e.Status == Domain.Enums.EnrollmentStatus.Active || e.Status == Domain.Enums.EnrollmentStatus.Completed), ct);
    }

    public async Task<IEnumerable<Guid>> GetStudentCourses(Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId && (e.Status == Domain.Enums.EnrollmentStatus.Active || e.Status == Domain.Enums.EnrollmentStatus.Completed))
            .Select(e => e.CourseId)
            .ToListAsync(ct);
    }
}
