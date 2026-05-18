using MediatR;
using StepLearning.Shared.Result;

namespace Commerce.Application.Payment.Features.Checkout;

public record CheckoutCommand(Guid StudentId, Guid CourseId) : IRequest<Result<string>>;
