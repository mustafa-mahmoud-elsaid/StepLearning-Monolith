namespace StepLearning.Shared.Abstraction;

public interface ICourseService
{
    Task<bool> Exists(Guid courseId, CancellationToken ct = default);
    Task<decimal?> GetPrice(Guid courseId, CancellationToken ct = default);
}
