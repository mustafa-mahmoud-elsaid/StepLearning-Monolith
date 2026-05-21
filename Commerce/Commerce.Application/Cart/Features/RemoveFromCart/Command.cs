using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.RemoveFromCart;

public sealed record RemoveFromCartCommand(Guid StudentId, Guid CourseId) : IRequest<Result>;
