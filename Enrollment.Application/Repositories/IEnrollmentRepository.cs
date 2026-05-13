namespace Enrollment.Application.Repositories;

public interface IEnrollmentRepository
{
    Task AddEnrollment(Domain.Entities.Enrollment enrollment, CancellationToken ct = default);
    Task<bool> IsEnrolled(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<Guid>> GetStudentCourses(Guid studentId, CancellationToken ct = default);
}
