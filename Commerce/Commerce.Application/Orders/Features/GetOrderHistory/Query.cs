using Commerce.Application.Orders.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Orders.Features.GetOrderHistory;

public record GetOrderHistoryQuery(Guid StudentId) : IRequest<Result<List<OrderDto>>>;
