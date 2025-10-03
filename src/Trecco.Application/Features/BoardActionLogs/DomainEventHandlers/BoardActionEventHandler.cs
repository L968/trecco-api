using Microsoft.AspNetCore.SignalR;
using Trecco.Application.Common.Authentication;
using Trecco.Application.Domain.BoardActionLogs;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;
using Trecco.Application.Infrastructure.Hubs;

namespace Trecco.Application.Features.BoardActionLogs.DomainEventHandlers;

internal sealed class BoardActionEventHandler
    : INotificationHandler<CardMovedDomainEvent>,
      INotificationHandler<ListAddedDomainEvent>,
      INotificationHandler<ListDeletedDomainEvent>
{
    private readonly IBoardActionLogRepository _boardLogRepository;
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly IUserContext _userContext;

    public BoardActionEventHandler(
        IBoardActionLogRepository logRepository,
        IHubContext<BoardHub> hubContext,
        IUserContext userContext)
    {
        _boardLogRepository = logRepository;
        _hubContext = hubContext;
        _userContext = userContext;
    }

    public async Task Handle(CardMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} moved card '{notification.CardTitle}' to list '{notification.TargetListName}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    public async Task Handle(ListAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} added a new list '{notification.ListName}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    public async Task Handle(ListDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} deleted list '{notification.ListName}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    private async Task SaveAndBroadcastAsync(Guid boardId, Guid userId, string details, CancellationToken cancellationToken)
    {
        var log = new BoardActionLog(boardId, userId, details);

        await _boardLogRepository.AddAsync(log, cancellationToken);

        await _hubContext.Clients
            .Group(boardId.ToString())
            .SendAsync("BoardLogged",
                log.Id,
                log.UserId,
                log.Details,
                log.Timestamp,
                cancellationToken
            );
    }

    private Guid EnsureUserId()
    {
        if (_userContext.UserId is null || _userContext.UserId == Guid.Empty)
        {
            throw new InvalidOperationException("Cannot log board action: UserId not available in context.");
        }

        return _userContext.UserId.Value;
    }

    private static string MaskUserId(Guid userId)
    {
        string str = userId.ToString();
        return str.Length > 7 ? string.Concat(str.AsSpan(0, 7), "...") : str;
    }
}
