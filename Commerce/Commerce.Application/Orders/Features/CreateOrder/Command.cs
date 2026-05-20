using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Orders.Features.CreateOrder;

public record CreateOrderCommand(Guid StudentId) : IRequest<Result<Guid>>;
