using Identity.Application.DTO;

namespace Identity.Application.Features.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
