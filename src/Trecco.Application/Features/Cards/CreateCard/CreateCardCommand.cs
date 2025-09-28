namespace Trecco.Application.Features.Cards.CreateCard;

public sealed record CreateCardCommand(
    Guid BoardId,
    Guid ListId,
    string Title,
    string Description,
    Guid RequesterId
) : IRequest<Result>;
