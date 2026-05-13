namespace StepLearning.Shared.Abstraction;

public interface IEnrollmentService
{
    Task<bool> IsEnrolled(Guid studentId, Guid courseId, CancellationToken ct = default);
}
