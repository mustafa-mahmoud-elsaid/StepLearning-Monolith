using Commerce.Application.Orders.DTO;
using Commerce.Application.Orders.Repositories;

namespace Commerce.Application.Orders.Features.GetOrder;

internal sealed class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null || order.StudentId != request.StudentId)
            return Result<OrderDto>.Failure("Order not found");

        var dto = new OrderDto(
            order.Id,
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.CourseId, i.CourseTitle, i.Price)).ToList()
        );

        return Result<OrderDto>.Success(dto);
    }
}
