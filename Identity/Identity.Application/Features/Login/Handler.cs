using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.JWT;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Login;

internal sealed class Handler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Credentials.Email);
        if (user is null)
            return Result<LoginResponse>.Failure("Invalid email or password");

        var result = await signInManager.CheckPasswordSignInAsync(user!, request.Credentials.Password, false);

        if (!result.Succeeded)
            return Result<LoginResponse>.Failure("Invalid email or password");


        var jwtToken = await tokenService.GenerateJWTToken(user!);

        var refTokenResult = await tokenService.GenerateRefreshToken(user, cancellationToken);

        if (!refTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(refTokenResult.Error!);

        

        return Result<LoginResponse>.Success(new(Token: jwtToken, refTokenResult.Value!));
    }
}

