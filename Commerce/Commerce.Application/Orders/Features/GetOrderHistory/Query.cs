using Commerce.Application.Orders.DTO;

namespace Commerce.Application.Orders.Features.GetOrderHistory;

public record GetOrderHistoryQuery(Guid StudentId) : IRequest<Result<List<OrderDto>>>;
