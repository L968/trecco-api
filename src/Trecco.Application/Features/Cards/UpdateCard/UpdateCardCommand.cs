namespace Trecco.Application.Features.Cards.UpdateCard;

public sealed record UpdateCardCommand(
    Guid BoardId,
    Guid CardId,
    string Title,
    string Description
) : IRequest<Result>;
