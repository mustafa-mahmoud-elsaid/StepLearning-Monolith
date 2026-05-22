using Identity.Application.Interfaces;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StepLearning.Shared.Result;

namespace Identity.Infrastructure.Services;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<Result<Guid>> RegisterUserAsync(string email, string password, string role, CancellationToken ct = default)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Result<Guid>.Failure("Email already exists");

        var appUser = ApplicationUser.Create(email);
        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
            return Result<Guid>.Failure("Failed to register the user");

        await userManager.AddToRoleAsync(appUser, role);
        return Result<Guid>.Success(appUser.Id);
    }

    public async Task<Result<Guid>> CheckCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<Guid>.Failure("Invalid email or password");

        var result = await signInManager.CheckPasswordSignInAsync(user, password, false);

        if (!result.Succeeded)
            return Result<Guid>.Failure("Invalid email or password");

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<Guid>> FindUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<Guid>.Failure("User not found.");

        return Result<Guid>.Success(user.Id);
    }
}
