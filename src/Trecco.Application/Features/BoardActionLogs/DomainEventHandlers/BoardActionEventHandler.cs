using Trecco.Application.Domain.BoardActionLogs;

namespace Trecco.Application.Features.BoardActionLogs.DomainEventHandlers;

internal sealed class BoardActionEventHandler : INotificationHandler<BoardActionDomainEvent>
{
    private readonly IBoardActionLogRepository _boardLogRepository;

    public BoardActionEventHandler(IBoardActionLogRepository logRepository)
    {
        _boardLogRepository = logRepository;
    }

    public async Task Handle(BoardActionDomainEvent notification, CancellationToken cancellationToken)
    {
        var log = new BoardActionLog(
            notification.BoardId,
            notification.UserId,
            notification.Details
        );

        await _boardLogRepository.AddAsync(log, cancellationToken);
    }
}
