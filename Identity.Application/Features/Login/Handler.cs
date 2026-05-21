using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.JWT;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Login;

public sealed class Handler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ITokenService _tokenService = tokenService;
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Credentials.Email);
        if (user is null)
            return Result<LoginResponse>.Failure("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user!, request.Credentials.Password, false);

        if (!result.Succeeded)
            return Result<LoginResponse>.Failure("Invalid email or password");


        var jwtToken = await _tokenService.GenerateJWTToken(user!);

        var refTokenResult = await _tokenService.GenerateRefreshToken(user, cancellationToken);

        if (!refTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(refTokenResult.Error!);

        

        return Result<LoginResponse>.Success(new(Token: jwtToken, refTokenResult.Value!));
    }
}
