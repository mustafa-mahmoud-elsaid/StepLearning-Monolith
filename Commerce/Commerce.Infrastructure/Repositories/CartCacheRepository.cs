using Commerce.Application.Cart.Repositories;
using Commerce.Domain.Cart;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Commerce.Infrastructure.Repositories;

internal sealed class CartCacheRepository(IDistributedCache cache) : ICartCacheRepository
{
    public async Task<Cart?> GetAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        var cartJson = await cache.GetStringAsync(ownerId, cancellationToken);

        if (cartJson is null) return null;

        return JsonSerializer.Deserialize<Cart>(cartJson);
    }

    public async Task RemoveAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(ownerId, cancellationToken);
    }

    public async Task SaveAsync(string ownerId, Cart cart, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromTicks(ttl.Ticks * 3 / 4),
            AbsoluteExpirationRelativeToNow = ttl,
        };

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        var cartJson = JsonSerializer.Serialize(cart, jsonOptions);

        await cache.SetStringAsync(ownerId, cartJson, cacheOptions, cancellationToken);
    }
}
