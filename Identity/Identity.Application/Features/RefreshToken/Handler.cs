using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Application.RepositoryInterfaces;

namespace Identity.Application.Features.RefreshToken;
internal sealed class Handler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the existing refresh token
        var existingToken = await refreshTokenRepository.FindByTokenAsync(request.RefreshToken, cancellationToken);

        if (existingToken is null)
            return Result<LoginResponse>.Failure("Invalid refresh token.");

        // 2. Validate it's still usable
        if (!existingToken.IsValid)
            return Result<LoginResponse>.Failure("Refresh token is expired or revoked.");

        // 3. Revoke the old token (token rotation)
        existingToken.Revoke();

        // 4. Find the user who owns this token
        var userResult = await identityService.FindUserByIdAsync(existingToken.UserId, cancellationToken);

        if (!userResult.IsSuccess)
            return Result<LoginResponse>.Failure("User not found.");

        var userId = userResult.Value;

        // 5. Generate new JWT + refresh token pair
        var newJwtToken = await tokenService.GenerateJWTToken(userId);
        var newRefreshTokenResult = await tokenService.GenerateRefreshToken(userId, cancellationToken);

        if (!newRefreshTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(newRefreshTokenResult.Error!);

        // 6. Persist the revocation + new token in one save
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new(newJwtToken, newRefreshTokenResult.Value!));
    }
}


