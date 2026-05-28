using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Commerce.Infrastructure.BackgroundJobs;

public sealed class CartSyncJob(
    ICartCacheRepository cartCacheRepository,
    ICartRepository cartRepository,
    ILogger<CartSyncJob> logger)
{
    private const int BatchSize = 50;
    private const int MinAgeMinutes = 3;
    private const string UserKeyPrefix = "cart:user:";

    [AutomaticRetry(Attempts = 2)]
    public async Task ExecuteAsync()
    {
        var dirtyKeys = await cartCacheRepository
            .PopDirtyKeysAsync(BatchSize, MinAgeMinutes);

        if (dirtyKeys.Count == 0)
        {
            logger.LogDebug("No dirty carts to sync");
            return;
        }

        logger.LogInformation(
            "Starting cart sync for {Count} dirty cart(s)",
            dirtyKeys.Count);

        foreach (var key in dirtyKeys)
        {
            try
            {
                await SyncCartAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to sync cart for key {CartKey}. Re-marking as dirty",
                    key);

                await cartCacheRepository.MarkDirtyAsync(key);
            }
        }
    }

    private async Task SyncCartAsync(string cartKey)
    {
        if (!TryExtractUserId(cartKey, out var userId))
        {
            logger.LogWarning("Could not extract userId from key {CartKey}, skipping", cartKey);
            return;
        }

        var cachedItems = await cartCacheRepository.GetAsync(cartKey);

        if (cachedItems is null || cachedItems.Count == 0)
        {
            logger.LogDebug("Cache empty for {CartKey}, skipping sync", cartKey);
            return;
        }

        var cart = await cartRepository.GetByStudentIdAsync(userId);

        var isNewCart = cart is null;

        cart ??= Domain.Cart.Cart.Create(userId);

        cart.Clear();

        foreach (var item in cachedItems)
        {
            cart.AddItem(item.CourseId, item.Price, item.CourseTitle);
        }

        if (isNewCart)
            await cartRepository.AddAsync(cart);
        else
            await cartRepository.SaveChangesAsync();

        logger.LogInformation(
            "Synced cart for user {UserId} with {ItemCount} item(s)",
            userId,
            cachedItems.Count);
    }

    private static bool TryExtractUserId(string cartKey, out Guid userId)
    {
        userId = Guid.Empty;

        if (!cartKey.StartsWith(UserKeyPrefix))
            return false;

        var userIdString = cartKey[UserKeyPrefix.Length..];

        return Guid.TryParse(userIdString, out userId);
    }
}
