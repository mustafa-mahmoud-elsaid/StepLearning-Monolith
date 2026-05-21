using Identity.Application.Domain.Entities;
using Identity.Application.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StepLearning.Shared.Result;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Infrastructure.JWT;

internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly IUserClaimsProvider _claimsProvider;

    public TokenService(UserManager<ApplicationUser> userManager, IRefreshTokenRepository refTokenRepository, IConfiguration configuration, IUserClaimsProvider claimsProvider)
    {
        _userManager = userManager;
        _refTokenRepository = refTokenRepository;
        _configuration = configuration;
        _claimsProvider = claimsProvider;
    }
    public async Task<string> GenerateJWTToken(ApplicationUser user)
    {
        if(user is null)
            throw new InvalidOperationException("Can not generate a token for null user");

        var userClaims = await _userManager.GetClaimsAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var rolesClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName??""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email?? ""),
        };
        claims.AddRange(userClaims);
        claims.AddRange(rolesClaims);

        var additionalClaims = await _claimsProvider.GetAdditionalClaimsAsync(user.Id);
        claims.AddRange(additionalClaims);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? throw new InvalidOperationException("No key configured")));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:DurationInMinutes", 15)),
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

    }

    public async Task<Result<string>> GenerateRefreshToken(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        if (user is null)
            return Result<string>.Failure("User cannot be null.");

        if(await _userManager.FindByIdAsync(user.Id.ToString()) is null)
            return Result<string>.Failure("invalid user");


        var randomNumber = new byte[64];

        var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        var token = Convert.ToBase64String(randomNumber);
        RefreshToken refToken;

        try
        {
            refToken = RefreshToken.Create(token,
                DateTime.UtcNow
                .AddDays(_configuration.GetValue("JWT:RefreshTokenDurationInDays", 7)),
                user.Id);

        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Failure(ex.Message);
        }

        await _refTokenRepository.AddAsync(refToken, cancellationToken);

        await _refTokenRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(refToken.Token);
    }
}
