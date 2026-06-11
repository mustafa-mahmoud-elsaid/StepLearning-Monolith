using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using Commerce.Application.Cart.ServicesInterfaces;
using StepLearning.Shared.Abstraction;

namespace Commerce.Application.Cart.Features.AddToCart;

internal sealed class Handler(
    ICartRepository cartRepository,
    ICartCacheRepository cartCacheRepository,
    ICartOwnerProvider ownerProvider,
    ICourseService courseService) : IRequestHandler<AddToCartCommand, Result>
{
    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var courseSnapshot = await courseService
            .GetSnapshot(request.CourseId, cancellationToken);

        if (courseSnapshot is null)
            return Result.Failure("Course not found or not available");

        var owner = ownerProvider.GetOwner();

        var exists = await cartCacheRepository.CartExistsAsync(owner.Key);

        if (exists || owner.IsGuest)
        {
            var cartItemDto = new CartItemDto(
                courseSnapshot.CourseId,
                courseSnapshot.Title,
                courseSnapshot.Price);

            await cartCacheRepository.AddOrUpdateAsync(
                owner.Key,
                cartItemDto,
                cancellationToken);

            if (!owner.IsGuest)
                await cartCacheRepository.MarkDirtyAsync(owner.Key);

            return Result.Success();
        }

        var cart = await cartRepository
            .GetByStudentIdAsync(owner.UserId!.Value, cancellationToken);

        var isNewCart = cart is null;

        cart ??= Domain.Cart.Cart.Create(owner.UserId!.Value);

        try
        {
            cart.AddItem(
                courseSnapshot.CourseId,
                courseSnapshot.Price,
                courseSnapshot.Title);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        if (isNewCart)
            await cartRepository.AddAsync(cart, cancellationToken);
        else
            await cartRepository.SaveChangesAsync(cancellationToken);

        var cartItemsDto = cart.Items
            .Select(
            item =>
            new CartItemDto(
                item.CourseId,
                item.CourseTitle,
                item.Price))
            .ToList();

        await cartCacheRepository
            .CacheCartAsync(owner.Key, cartItemsDto, cancellationToken);

        return Result.Success();
    }
}
