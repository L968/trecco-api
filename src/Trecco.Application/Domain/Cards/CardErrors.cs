namespace Trecco.Application.Domain.Cards;

internal static class CardErrors
{
    public static Error NotFound(Guid cardId) =>
        Error.NotFound(
            "Card.NotFound",
            $"Card with id \"{cardId}\" was not found."
        );
}

