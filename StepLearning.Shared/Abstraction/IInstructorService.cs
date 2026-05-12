namespace StepLearning.Shared.Abstraction;

public interface IInstructorService
{
    Task<bool> Exists(Guid instructorId, CancellationToken ct = default);
}
