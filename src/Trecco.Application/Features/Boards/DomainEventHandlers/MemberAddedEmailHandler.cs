namespace Trecco.Application.Features.Boards.DomainEventHandlers;

internal sealed class MemberAddedEmailHandler(
    ILogger<MemberAddedEmailHandler> logger
    ) : INotificationHandler<MemberAddedDomainEvent>
{
    public Task Handle(MemberAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "DomainEvent: Simulating sending welcome email to user {UserId} for joining board {BoardId}.",
            notification.UserId,
            notification.BoardId
        );

        return Task.CompletedTask;
    }
}
