namespace StepLearning.Shared.Events;

public record PaymentSucceededEvent(Guid StudentId, Guid PaymentId, List<Guid> CourseIds);
