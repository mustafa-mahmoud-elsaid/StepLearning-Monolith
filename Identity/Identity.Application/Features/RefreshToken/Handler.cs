using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.JWT;
using Identity.Application.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.RefreshToken;
internal sealed class Handler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
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
        var user = await userManager.FindByIdAsync(existingToken.UserId.ToString());

        if (user is null)
            return Result<LoginResponse>.Failure("User not found.");

        // 5. Generate new JWT + refresh token pair
        var newJwtToken = await tokenService.GenerateJWTToken(user);
        var newRefreshTokenResult = await tokenService.GenerateRefreshToken(user, cancellationToken);

        if (!newRefreshTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(newRefreshTokenResult.Error!);

        // 6. Persist the revocation + new token in one save
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new(newJwtToken, newRefreshTokenResult.Value!));
    }
}


