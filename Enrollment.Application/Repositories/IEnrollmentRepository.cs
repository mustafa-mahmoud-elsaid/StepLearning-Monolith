namespace Enrollment.Application.Repositories;

public interface IEnrollmentRepository
{
    Task AddEnrollment(Domain.Entities.Enrollment enrollment, CancellationToken ct = default);
}
