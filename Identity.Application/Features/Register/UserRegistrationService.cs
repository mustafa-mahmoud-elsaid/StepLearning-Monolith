using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using Identity.Application.Infrastructure.JWT;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Register;

internal sealed class UserRegistrationService(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService)
{
    public async Task<Result<ApplicationUser>> CreateUserAsync(
        string email, string password, string role, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<ApplicationUser>.Failure("Email already exists");

        var appUser = ApplicationUser.Create(email);
        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
            return Result<ApplicationUser>.Failure("Failed to register the user");

        await userManager.AddToRoleAsync(appUser, role);
        return Result<ApplicationUser>.Success(appUser);
    }

    public async Task<Result<LoginResponse>> GenerateTokensAsync(ApplicationUser user, CancellationToken ct)
    {
        var jwtToken = await tokenService.GenerateJWTToken(user);
        var refTokenResult = await tokenService.GenerateRefreshToken(user, ct);

        if (!refTokenResult.IsSuccess)
            return Result<LoginResponse>.Failure(refTokenResult.Error!);

        return Result<LoginResponse>.Success(new LoginResponse(jwtToken, refTokenResult.Value!));
    }
}
