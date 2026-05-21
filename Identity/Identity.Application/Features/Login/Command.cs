using Identity.Application.Domain.DTO;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Login;

public record LoginCommand(LoginDto Credentials) : IRequest<Result<LoginResponse>>;
