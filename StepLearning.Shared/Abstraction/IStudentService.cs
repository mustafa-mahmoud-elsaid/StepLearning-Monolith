namespace StepLearning.Shared.Abstraction;

public interface IStudentService
{
    Task<bool> Exists(Guid studentId, CancellationToken ct = default);
}
