using Commerce.Application.Orders.DTO;

namespace Commerce.Application.Orders.Features.GetOrder;

public record GetOrderQuery(Guid OrderId, Guid StudentId) : IRequest<Result<OrderDto>>;
