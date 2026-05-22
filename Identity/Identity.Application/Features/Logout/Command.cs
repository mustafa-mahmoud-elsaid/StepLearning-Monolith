
namespace Identity.Application.Features.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;
