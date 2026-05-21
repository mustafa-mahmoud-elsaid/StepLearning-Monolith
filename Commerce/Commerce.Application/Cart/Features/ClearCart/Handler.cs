using Commerce.Application.Cart.Repositories;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.ClearCart;

internal sealed class Handler(ICartRepository cartRepository) : IRequestHandler<ClearCartCommand, Result>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        if (cart is null)
            return Result.Success();

        cart.Clear();
        await cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
