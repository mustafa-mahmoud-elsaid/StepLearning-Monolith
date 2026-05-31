namespace Identity.Application.DTO;

public record LoginResponse(string Token, string RefreshToken, Guid UserId);
