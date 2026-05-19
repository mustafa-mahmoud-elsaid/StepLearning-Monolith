using Commerce.Application.Cart.Repositories;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.RemoveFromCart;

internal sealed class Handler(ICartRepository cartRepository) : IRequestHandler<RemoveFromCartCommand, Result>
{
    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result.Failure("Student id must not be empty");

        if (request.CourseId == Guid.Empty)
            return Result.Failure("Course id must not be empty");

        var cart = await cartRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

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

        return Result.Success();
    }
}
