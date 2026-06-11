using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using Commerce.Domain.Cart;
using MassTransit.Initializers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Commerce.Infrastructure.Repositories;

internal sealed class CartCacheRepository : ICartCacheRepository
{
    private const string DirtyCartsKey = "dirty-carts";

    private readonly IDistributedCache _ditributedCache;
    private readonly int _expirationDays;
    private readonly ILogger<CartCacheRepository> _logger;
    private readonly IDatabase _redis;

    public CartCacheRepository(
        IDistributedCache ditributedCache,
        IConnectionMultiplexer redis,
        IConfiguration configuration,
        ILogger<CartCacheRepository> logger)
    {
        _ditributedCache = ditributedCache;
        _expirationDays = configuration.GetValue<int>("RedisSettings:CartExpirationDays");

        if (_expirationDays <= 0)
        {
            logger.LogWarning(
             "Invalid or missing configuration for {ConfigKey}. " +
             "Using default value of {DefaultDays} days.",
             "RedisSettings:CartExpirationDays",
             7);

            _expirationDays = 7;
        }

        _logger = logger;
        _redis = redis.GetDatabase();
    }

    public async Task<List<CartItemDto>?> GetAsync(string cartKey, CancellationToken cancellationToken = default)
    {
        var values =
            await _redis.HashValuesAsync(cartKey);

        if (values is null)
            return null;

        return values
            .Select(v =>
            JsonSerializer.Deserialize<CartItemDto>(v.ToString())!)
            .ToList();

    }

    public async Task RemoveItemAsync(string cartKey, string courseId, CancellationToken cancellationToken = default)
    {
        var deleteTask = _redis.HashDeleteAsync(cartKey, courseId);

        var expireTask = _redis.KeyExpireAsync(
            cartKey,
            TimeSpan.FromDays(_expirationDays));

        await Task.WhenAll(deleteTask, expireTask);
    }

    public async Task AddOrUpdateAsync(string cartKey, CartItemDto item, CancellationToken cancellationToken = default)
    {
        var hashTask = _redis.HashSetAsync(
        cartKey,
        item.CourseId.ToString(),
        JsonSerializer.Serialize(item));

        var expireTask = _redis.KeyExpireAsync(
            cartKey,
            TimeSpan.FromDays(_expirationDays));

        await Task.WhenAll(hashTask, expireTask);
    }

    public async Task CacheCartAsync(
        string cartKey,
        IReadOnlyCollection<CartItemDto> items,
        CancellationToken cancellationToken = default)
    {
        var transaction = _redis.CreateTransaction();

        _ = transaction.KeyDeleteAsync(cartKey);

        if (items.Count > 0)
        {
            var hashEntries = items.Select(item =>
            {
                return new HashEntry(item.CourseId.ToString(), JsonSerializer.Serialize(item));
            }).ToArray();

            _ = transaction.HashSetAsync(
                cartKey,
                hashEntries);

        }

        _ = transaction.KeyExpireAsync(
            cartKey,
            TimeSpan.FromDays(_expirationDays));

        await transaction.ExecuteAsync();

    }
    public async Task<bool> CartExistsAsync(string cartKey)
    {
        return await _redis.KeyExistsAsync(cartKey);
    }

    public async Task RemoveCartAsync(string cartKey)
    {
        await _redis.KeyDeleteAsync(cartKey);
    }

    public async Task<bool> MigrateCartAsync(string sourceKey, string destinationKey)
    {
        if (!await _redis.KeyExistsAsync(sourceKey))
            return false;

        await _redis.KeyRenameAsync(sourceKey, destinationKey);

        await _redis.KeyExpireAsync(
            destinationKey,
            TimeSpan.FromDays(_expirationDays));

        return true;
    }

    public async Task MarkDirtyAsync(string cartKey)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        await _redis.SortedSetAddAsync(DirtyCartsKey, cartKey, timestamp);
    }

    public async Task RemoveDirtyAsync(string cartKey)
    {
        await _redis.SortedSetRemoveAsync(DirtyCartsKey, cartKey);
    }

    public async Task<List<string>> PopDirtyKeysAsync(int count, int minAgeMinutes)
    {
        var cutoff = DateTimeOffset.UtcNow
            .AddMinutes(-minAgeMinutes)
            .ToUnixTimeSeconds();

        var entries = await _redis.SortedSetRangeByScoreAsync(
            DirtyCartsKey,
            stop: cutoff,
            take: count);

        if (entries.Length == 0)
            return [];

        var keys = entries
            .Select(e => e.ToString())
            .ToList();

        var redisValues = entries
            .ToArray();

        await _redis.SortedSetRemoveAsync(DirtyCartsKey, redisValues);

        return keys;
    }
}
