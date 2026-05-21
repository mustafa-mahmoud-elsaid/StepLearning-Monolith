using Commerce.Application.Cart.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Cart.Features.GetCart;

public sealed record GetCartQuery(Guid StudentId) : IRequest<Result<CartDto>>;
