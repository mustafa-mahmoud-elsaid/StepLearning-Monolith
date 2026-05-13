namespace StepLearning.Shared.Events;

public record PaymentSucceededEvent(Guid StudentId, Guid CourseId, Guid PaymentId);
