using MediatR;
using StepLearning.Shared.Result;

namespace Payment.Application.Features.Checkout;

public record CheckoutCommand(Guid CourseId) : IRequest<Result<Guid>>;
