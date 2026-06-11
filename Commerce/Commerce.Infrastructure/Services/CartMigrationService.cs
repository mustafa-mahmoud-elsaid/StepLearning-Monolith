using Commerce.Application.Cart.Repositories;
using Microsoft.Extensions.Logging;
using StepLearning.Shared.Abstraction;

namespace Commerce.Infrastructure.Services;

internal sealed class CartMigrationService(
    ICartCacheRepository cartCacheRepository,
    ILogger<CartMigrationService> logger) : ICartMigrationService
{
    public async Task<bool> MigrateGuestCartAsync(string guestCartKey, Guid userId, CancellationToken ct = default)
    {
        var userCartKey = $"cart:user:{userId}";

        var migrated = await cartCacheRepository.MigrateCartAsync(guestCartKey, userCartKey);

        if (migrated)
        {
            await cartCacheRepository.MarkDirtyAsync(userCartKey);

            logger.LogInformation(
                "Migrated guest cart {GuestKey} to user cart {UserKey}",
                guestCartKey,
                userCartKey);
        }
        else
        {
            logger.LogDebug(
                "No guest cart found at {GuestKey} for user {UserId}",
                guestCartKey,
                userId);
        }

        return migrated;
    }
}
