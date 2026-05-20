using Commerce.Application.Cart.Repositories;
using Commerce.Application.Orders.Domain.Entities;
using Commerce.Application.Orders.Repositories;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Orders.Features.CreateOrder;

internal sealed class Handler(
    ICartRepository cartRepository,
    IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.StudentId == Guid.Empty)
            return Result<Guid>.Failure("Student id must not be empty");

        var cart = await cartRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        if (cart is null || !cart.Items.Any())
            return Result<Guid>.Failure("Cart is empty");

        var orderItems = cart.Items.Select(item =>
            OrderItem.Create(Guid.Empty, item.CourseId, item.CourseTitle, item.Price)).ToList();

        var order = Order.Create(request.StudentId, orderItems);

        await orderRepository.AddAsync(order, cancellationToken);

        cart.Clear();
        await cartRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}
