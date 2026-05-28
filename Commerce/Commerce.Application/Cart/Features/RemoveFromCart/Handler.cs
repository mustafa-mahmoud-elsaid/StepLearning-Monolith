using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;

namespace Commerce.Application.Cart.Features.RemoveFromCart;

internal sealed class Handler(
    ICartRepository cartRepository,
    ICartCacheRepository cartCacheRepository,
    ICartOwnerProvider ownerProvider) : IRequestHandler<RemoveFromCartCommand, Result>
{
    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var owner = ownerProvider.GetOwner();

        var cacheExists = await cartCacheRepository.CartExistsAsync(owner.Key);

        if (cacheExists)
        {
            await cartCacheRepository.RemoveItemAsync(
                owner.Key,
                request.CourseId.ToString(),
                cancellationToken);

            if (!owner.IsGuest)
                await cartCacheRepository.MarkDirtyAsync(owner.Key);

            return Result.Success();
        }

        // Cache miss – remove from DB, then cache the updated cart
        if (owner.IsGuest)
            return Result.Failure("Cart not found");

        var cart = await cartRepository.GetByStudentIdAsync(owner.UserId!.Value, cancellationToken);

        if (cart is null)
            return Result.Failure("Cart not found");

        try
        {
            cart.RemoveItem(request.CourseId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await cartRepository.SaveChangesAsync(cancellationToken);

        var cartItemsDto = cart.Items
            .Select(item =>
            new CartItemDto(item.CourseId, item.CourseTitle, item.Price))
            .ToList();

        await cartCacheRepository.CacheCartAsync(
            owner.Key,
            cartItemsDto,
            cancellationToken);

        return Result.Success();
    }
}
