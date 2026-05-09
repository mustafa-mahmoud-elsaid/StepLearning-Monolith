namespace Identity.Application.Domain.DTO;

public record LoginResponse(string Token, string RefreshToken);
