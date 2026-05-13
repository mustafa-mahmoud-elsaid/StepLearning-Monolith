namespace StepLearning.Shared.Abstraction;

public interface IEnrollmentService
{
    Task<bool> IsEnrolled(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IEnumerable<Guid>> GetStudentCourses(Guid studentId, CancellationToken ct = default);
}
