using MassTransit;
using StepLearning.Shared.Abstraction;

namespace Host.Api.Messaging;

public sealed class MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        return publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
