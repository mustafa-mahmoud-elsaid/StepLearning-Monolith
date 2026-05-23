namespace Commerce.Application.Cart.Repositories;

public interface ICartCacheRepository
{
    Task<Domain.Cart.Cart?> GetAsync(string ownerId, CancellationToken cancellationToken = default);

    Task SaveAsync(string ownerId, Domain.Cart.Cart cart, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task RemoveAsync(string ownerId, CancellationToken cancellationToken = default);
}
