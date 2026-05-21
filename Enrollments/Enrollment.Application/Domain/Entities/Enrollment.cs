using Enrollment.Application.Domain.Enums;

namespace Enrollment.Application.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid PaymentId { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    public static Enrollment Create(Guid studentId, Guid courseId, Guid paymentId, EnrollmentStatus status)
    {
        if (studentId == Guid.Empty)
            throw new InvalidOperationException("Student id can not be empty");

        if (courseId == Guid.Empty)
            throw new InvalidOperationException("Course id can not be empty");

        if (paymentId == Guid.Empty)
            throw new InvalidOperationException("Payment id can not be empty");

        return new()
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            PaymentId = paymentId,
            EnrolledAt = DateTime.UtcNow,
            Status = status
        };
    }
}
