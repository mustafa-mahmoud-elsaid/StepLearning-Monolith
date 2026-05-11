namespace Identity.Application.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public bool IsRevoked { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public Guid UserId { get; private set; }

    public static RefreshToken Create(string token, DateTime expireAt, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("refresh token must be valid to create");
        if (userId == Guid.Empty)
            throw new InvalidOperationException("user id must not be empty");
        if (expireAt <= DateTime.UtcNow)
            throw new InvalidOperationException("expires at must be a future date");

        return new()
        {
            Id = Guid.NewGuid(),
            Token = token,
            IsRevoked = false,
            ExpiresAt = expireAt,
            UserId = userId
        };
    }

    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public void Revoke()
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token is already revoked.");

        IsRevoked = true;
    }
}
