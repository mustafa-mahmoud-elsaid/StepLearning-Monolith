namespace StepLearning.Shared.Events;

public record NotificationRequestedEvent(
    Guid NotificationId,
    string Channel,
    string TemplateKey,
    string To,
    string Subject,
    Dictionary<string, string> Data,
    Guid CorrelationId,
    DateTime OccurredAtUtc);
