using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure;

public class ApplicationUser : IdentityUser<Guid>
{
    public static ApplicationUser Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("Email must have a value");

        email = email.Trim();
        return new()
        {
            Email = email,
            UserName = email
        };
    }
}
