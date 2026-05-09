using Identity.Application.Domain.DTO;
using Identity.Application.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StepLearning.Shared.Result;

namespace Identity.Application.Features.Login;

public sealed class Handler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public Handler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        this._signInManager = signInManager;
    }
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.dto.Email);
        if (user is null)
            return Result<LoginResponse>.Failure("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user!, request.dto.Password, false);

        if(!result.Succeeded) 
            return Result<LoginResponse>.Failure("Invalid email or password");

        return Result<LoginResponse>.Success(new("token", "refreshToken"));
    }
}
