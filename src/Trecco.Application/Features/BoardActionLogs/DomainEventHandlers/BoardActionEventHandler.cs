using Trecco.Application.Common.Abstractions;
using Trecco.Application.Common.Authentication;
using Trecco.Domain.BoardActionLogs;
using Trecco.Domain.Cards;
using Trecco.Domain.Lists;

namespace Trecco.Application.Features.BoardActionLogs.DomainEventHandlers;

internal sealed class BoardActionEventHandler(
    IUserContext userContext,
    IBoardNotifier boardNotifier,
    IBoardActionLogRepository boardLogRepository
) :
    INotificationHandler<CardMovedDomainEvent>,
    INotificationHandler<MemberAddedDomainEvent>,
    INotificationHandler<ListAddedDomainEvent>,
    INotificationHandler<ListRenamedDomainEvent>,
    INotificationHandler<ListDeletedDomainEvent>
{
    public async Task Handle(CardMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} moved card '{notification.CardTitle}' to '{notification.TargetListName}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    public async Task Handle(MemberAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string maskedMember = MaskUserId(notification.UserId);
        string logDetails = $"User {maskedUser} added member '{maskedMember}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    public async Task Handle(ListAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} added a new list '{notification.ListName}'";

        await SaveAndBroadcastAsync(notification.BoardId, userId, logDetails, cancellationToken);
    }

    public async Task Handle(ListRenamedDomainEvent notification, CancellationToken cancellationToken)
    {
        Guid userId = EnsureUserId();
        string maskedUser = MaskUserId(userId);
        string logDetails = $"User {maskedUser} renamed list '{notification.OldName}' to '{notification.NewName}'";

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

        await boardLogRepository.AddAsync(log, cancellationToken);

        await boardNotifier.BroadcastBoardLogAsync(
            boardId,
            log.Id,
            log.UserId,
            log.Details,
            log.Timestamp,
            cancellationToken
        );
    }

    private Guid EnsureUserId()
    {
        if (userContext.UserId is null || userContext.UserId == Guid.Empty)
        {
            throw new InvalidOperationException("Cannot log board action: UserId not available in context.");
        }

        return userContext.UserId.Value;
    }

    private static string MaskUserId(Guid userId)
    {
        string str = userId.ToString();
        return str.Length > 7 ? string.Concat(str.AsSpan(0, 7), "...") : str;
    }
}
