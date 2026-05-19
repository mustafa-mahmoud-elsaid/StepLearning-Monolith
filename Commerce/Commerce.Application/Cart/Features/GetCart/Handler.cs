using Commerce.Application.Cart.DTO;
using Commerce.Application.Cart.Repositories;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.GetCart;

internal sealed class Handler(ICartRepository cartRepository) : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result<CartDto>.Failure("Student id must not be empty");

        var cart = await cartRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        if (cart is null)
            return Result<CartDto>.Success(new CartDto(request.StudentId, [], 0));

        var items = cart.Items
            .Select(item => new CartItemDto(item.CourseId, item.CourseTitle, item.Price))
            .ToList();

        return Result<CartDto>.Success(new CartDto(
            cart.StudentId,
            items,
            items.Sum(item => item.Price)));
    }
}
