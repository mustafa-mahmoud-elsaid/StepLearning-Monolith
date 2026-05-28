using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;
using Commerce.Domain.Cart;

namespace Commerce.Application.Cart.Features.GetCart;

internal sealed class Handler(
    ICartRepository cartRepository,
    ICartCacheRepository cartCacheRepository,
    ICartOwnerProvider cartOwnerProvider)
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var owner = cartOwnerProvider.GetOwner();

        var cachedCart =
            await cartCacheRepository.GetAsync(
                owner.Key,
                cancellationToken);

        if (cachedCart is not null)
            return CartSuccessResult(cachedCart);

        if (owner.IsGuest)
            return EmptyCart();

        var persistedCart =
            await cartRepository.GetByStudentIdAsync(
                owner.UserId!.Value,
                cancellationToken);

        if (persistedCart is null)
            return EmptyCart();

        await cartCacheRepository.CacheCartAsync(
            owner.Key,
            persistedCart.Items.Select(item =>
            new CartItemDto(item.CourseId, item.CourseTitle, item.Price)).ToList(),
            cancellationToken);

        return CartSuccessResult(
            persistedCart.Items.ToList());
    }
    private static Result<CartDto> EmptyCart()
    {
        return Result<CartDto>.Success(
            new CartDto([], 0));
    }
    private static Result<CartDto> CartSuccessResult(List<CartItem> items)
    {
        var itemsDto = items
            .Select(i => 
            new CartItemDto(i.CourseId, i.CourseTitle, i.Price))
            .ToList();

        return Result<CartDto>.Success(
            new(itemsDto,
            items
            .Sum(i => i.Price)));
    }
}
