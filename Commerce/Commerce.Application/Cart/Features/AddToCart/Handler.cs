using Commerce.Application.Cart.Repositories;
using MediatR;
using StepLearning.Shared.Abstraction;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.AddToCart;

internal sealed class Handler(
    ICartRepository cartRepository,
    ICourseService courseService) : IRequestHandler<AddToCartCommand, Result>
{
    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result.Failure("Student id must not be empty");

        if (request.CourseId == Guid.Empty)
            return Result.Failure("Course id must not be empty");

        var courseSnapshot = await courseService.GetSnapshot(request.CourseId, cancellationToken);

        if (courseSnapshot is null)
            return Result.Failure("Course not found or not available");

        var cart = await cartRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
        var isNewCart = cart is null;

        cart ??= Domain.Entities.Cart.Create(request.StudentId);

        try
        {
            cart.AddItem(courseSnapshot.CourseId, courseSnapshot.Price, courseSnapshot.Title);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        if (isNewCart)
            await cartRepository.AddAsync(cart, cancellationToken);
        else
            await cartRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
