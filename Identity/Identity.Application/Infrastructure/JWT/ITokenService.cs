using StepLearning.Shared.Result;

namespace Identity.Application.Infrastructure.JWT;

public interface ITokenService
{
    Task<string> GenerateJWTToken(ApplicationUser user);

    /// <summary>
    /// Generates a refresh token for the specified user and stores it in the database.
    /// </summary>
    /// <param name="user">The user for whom the refresh token will be generated.</param>
    /// <param name="cancellationToken"></param>
    /// <returns> 
    /// A <see cref="Result{T}"/> containing the generated refresh token if the operation succeeds;
    /// otherwise, a failed result describing the error.
    /// </returns>
    Task<Result<string>> GenerateRefreshToken(ApplicationUser user, CancellationToken cancellationToken = default);
}
