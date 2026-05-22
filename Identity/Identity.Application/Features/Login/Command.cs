using Identity.Application.DTO;

namespace Identity.Application.Features.Login;

public record LoginCommand(LoginDto Credentials) : IRequest<Result<LoginResponse>>;
