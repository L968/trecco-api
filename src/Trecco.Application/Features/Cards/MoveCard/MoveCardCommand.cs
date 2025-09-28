namespace Trecco.Application.Features.Cards.MoveCard;

public sealed record MoveCardCommand(
    Guid BoardId,
    Guid CardId,
    Guid TargetListId,
    int TargetPosition,
    Guid RequesterId
) : IRequest<Result>;
