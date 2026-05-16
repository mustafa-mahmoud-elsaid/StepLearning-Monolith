namespace StepLearning.Shared.Events;

public record EnrollmentCompletedEvent(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    Guid PaymentId,
    DateTime OccurredAtUtc);
