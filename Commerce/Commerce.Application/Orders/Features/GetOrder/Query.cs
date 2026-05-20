using Commerce.Application.Orders.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Orders.Features.GetOrder;

public record GetOrderQuery(Guid OrderId, Guid StudentId) : IRequest<Result<OrderDto>>;
