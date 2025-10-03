using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.DomainEventHandlers;

internal sealed class MemberAddedEmailHandler : INotificationHandler<MemberAddedDomainEvent>
{
    private readonly ILogger<MemberAddedEmailHandler> _logger;

    public MemberAddedEmailHandler(ILogger<MemberAddedEmailHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MemberAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "DomainEvent: Simulating sending welcome email to user {UserId} for joining board {BoardId}.",
            notification.UserId,
            notification.BoardId
        );

        return Task.CompletedTask;
    }
}
