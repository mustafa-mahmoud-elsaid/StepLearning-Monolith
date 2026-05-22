namespace Courses.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Entity is already deleted.");

        IsDeleted = true;
    }
}
