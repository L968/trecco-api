namespace Trecco.Application.Features.Cards.DeleteCard;

public sealed record DeleteCardCommand(
    Guid BoardId,
    Guid CardId
) : IRequest<Result>;
