using StepLearning.Shared.Result;

namespace Identity.Application.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterUserAsync(string email, string password, string role, CancellationToken ct = default);
    Task<Result<Guid>> CheckCredentialsAsync(string email, string password, CancellationToken ct = default);
    Task<Result<Guid>> FindUserByIdAsync(Guid userId, CancellationToken ct = default);
}
