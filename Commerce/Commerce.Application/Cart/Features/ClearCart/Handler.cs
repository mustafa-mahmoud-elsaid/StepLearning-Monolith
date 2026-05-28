using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;
using Microsoft.Extensions.Logging;

namespace Commerce.Application.Cart.Features.ClearCart;

internal sealed class Handler(
    ICartRepository cartRepository,
    ICartCacheRepository cartCacheRepository,
    ICartOwnerProvider ownerProvider,
    ILogger<Handler> logger) : IRequestHandler<ClearCartCommand, Result>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var owner = ownerProvider.GetOwner();

        await cartCacheRepository.RemoveCartAsync(owner.Key);

        logger.LogInformation("Owner: {ownerKey} cart has been removed from the cache",
            owner.Key);


        var cart = await cartRepository.GetByStudentIdAsync(owner.UserId!.Value, cancellationToken);

        if (cart is null)
            return Result.Success();

        cart.Clear();
        await cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
