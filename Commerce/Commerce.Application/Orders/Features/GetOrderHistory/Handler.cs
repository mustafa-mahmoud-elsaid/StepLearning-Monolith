using Commerce.Application.Orders.DTO;
using Commerce.Application.Orders.Repositories;

namespace Commerce.Application.Orders.Features.GetOrderHistory;

internal sealed class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrderHistoryQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        var dtos = orders.Select(order => new OrderDto(
            order.Id,
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.CourseId, i.CourseTitle, i.Price)).ToList()
        )).ToList();

        return Result<List<OrderDto>>.Success(dtos);
    }
}
