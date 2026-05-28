using Commerce.Application.Cart.DTO;
using Commerce.Domain.Cart;

namespace Commerce.Application.Cart.Repositories;

public interface ICartCacheRepository
{
    Task<List<CartItem>?> GetAsync(string cartKey, CancellationToken cancellationToken = default);

    Task AddOrUpdateAsync(string cartKey,CartItemDto item, CancellationToken cancellationToken = default);
    Task CacheCartAsync(string cartKey, IReadOnlyCollection<CartItemDto> items, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(string cartKey, string courseId, CancellationToken cancellationToken = default);
    Task RemoveCartAsync(string cartKey);
    Task<bool> CartExistsAsync(string cartKey);
}
