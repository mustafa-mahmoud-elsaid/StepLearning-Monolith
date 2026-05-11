using Identity.Application.Domain.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
