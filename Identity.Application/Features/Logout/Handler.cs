using Identity.Application.RepositoryInterfaces;
using MediatR;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Logout;

public sealed class Handler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public Handler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var stored = await _refreshTokenRepository.FindByTokenAsync(request.RefreshToken, cancellationToken);

        if (stored is null)
            return Result.Failure("Invalid refresh token.");

        if (stored.IsRevoked)
            return Result.Success();

        stored.Revoke();
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
