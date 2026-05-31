namespace StepLearning.Shared.Abstraction;

public interface ICartMigrationService
{
    Task<bool> MigrateGuestCartAsync(string guestCartKey, Guid userId, CancellationToken ct = default);
}
