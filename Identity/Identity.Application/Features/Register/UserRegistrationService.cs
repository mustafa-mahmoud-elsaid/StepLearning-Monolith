using Identity.Application.DTO;
using Identity.Application.Interfaces;

namespace Identity.Application.Features.Register;

internal sealed class UserRegistrationService(
    IIdentityService identityService,
    ITokenService tokenService)
{
    public async Task<Result<Guid>> CreateUserAsync(
        string email, string password, string role, CancellationToken ct)
    {
        return await identityService.RegisterUserAsync(email, password, role, ct);
    }

    public async Task<Result<LoginResponse>> GenerateTokensAsync(Guid userId, CancellationToken ct)
    {
        var jwtToken = await tokenService.GenerateJWTToken(userId);
        var refTokenResult = await tokenService.GenerateRefreshToken(userId, ct);

        if (!refTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(refTokenResult.Error!);

        return Result<LoginResponse>.Success(new LoginResponse(jwtToken, refTokenResult.Value!));
    }

}
