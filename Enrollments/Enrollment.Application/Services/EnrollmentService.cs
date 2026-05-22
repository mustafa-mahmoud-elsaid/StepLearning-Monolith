using Enrollment.Application.Repositories;
using StepLearning.Shared.Abstraction;

namespace Enrollment.Application.Services;

internal class EnrollmentService(IEnrollmentRepository enrollmentRepository) : IEnrollmentService
{
    public async Task<bool> IsEnrolled(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        if (studentId == Guid.Empty || courseId == Guid.Empty)
            return false;

        return await enrollmentRepository.IsEnrolled(studentId, courseId, ct);
    }

    public async Task<IEnumerable<Guid>> GetStudentCourses(Guid studentId, CancellationToken ct = default)
    {
        if (studentId == Guid.Empty)
            return [];

        return await enrollmentRepository.GetStudentCourses(studentId, ct);
    }
}
