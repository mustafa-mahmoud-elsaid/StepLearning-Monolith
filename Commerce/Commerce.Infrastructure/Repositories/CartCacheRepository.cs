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

    public async Task<List<CartItem>?> GetAsync(string cartKey, CancellationToken cancellationToken = default)
    {
        var values =
            await _redis.HashValuesAsync(cartKey);

        if (values is null)
            return null;

        return values
            .Select(v =>
            JsonSerializer.Deserialize<CartItem>(v.ToString())!)
            .ToList();

    }

    public async Task RemoveItemAsync(string cartKey, string courseId, CancellationToken cancellationToken = default)
    {
        await _redis.HashDeleteAsync(
            cartKey,
            courseId);
    }

    public async Task AddOrUpdateAsync(string cartKey, CartItemDto item, CancellationToken cancellationToken = default)
    {
        await _redis.HashSetAsync(
            cartKey,
            item.CourseId.ToString(),
            JsonSerializer.Serialize(item));

        await _redis.KeyExpireAsync(
        cartKey,
        TimeSpan.FromDays(_expirationDays));
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
}
