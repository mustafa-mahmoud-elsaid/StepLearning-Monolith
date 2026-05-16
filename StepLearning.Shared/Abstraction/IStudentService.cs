namespace StepLearning.Shared.Abstraction;

public interface IStudentService
{
    Task<bool> Exists(Guid studentId, CancellationToken ct = default);
    Task<string?> GetEmail(Guid studentId, CancellationToken ct = default);
}
