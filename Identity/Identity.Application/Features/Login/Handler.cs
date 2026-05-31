using Identity.Application.DTO;
using Identity.Application.Interfaces;

namespace Identity.Application.Features.Login;

internal sealed class Handler(
    IIdentityService identityService,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authResult = await identityService.CheckCredentialsAsync(request.Credentials.Email, request.Credentials.Password, cancellationToken);
        if (!authResult.IsSuccess)
            return Result<LoginResponse>.Failure(authResult.Error!);

        var userId = authResult.Value;

        var jwtToken = await tokenService.GenerateJWTToken(userId);

        var refTokenResult = await tokenService.GenerateRefreshToken(userId, cancellationToken);

        if (!refTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(refTokenResult.Error!);

        

        return Result<LoginResponse>.Success(new(jwtToken, refTokenResult.Value!, userId));
    }
}

