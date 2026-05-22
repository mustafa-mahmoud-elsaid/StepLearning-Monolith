namespace Identity.Domain.Entities;

public class Instructor
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? ProfilePictureUrl { get; private set; }

    public string FullName => $"{FirstName} {LastName}";
    public string? Bio { get; private set; }

    public static Instructor Create(string firstName, string lastName, Guid userId, string? profilePictureUrl, string? bio)
    {
        if(string.IsNullOrWhiteSpace(firstName) 
            || string.IsNullOrWhiteSpace(lastName))
        {
            throw new InvalidOperationException("FirstName and LastName must have actual value");
        }
        if(userId == Guid.Empty)
        {
            throw new InvalidOperationException("User id must not be empty");
        }
        return new()
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            UserId = userId,
            Bio = bio,
            ProfilePictureUrl = profilePictureUrl,
        };
    }

}
