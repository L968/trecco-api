namespace Trecco.Application.Features.Cards.MoveCard;

internal sealed class MoveCardValidator : AbstractValidator<MoveCardCommand>
{
    public MoveCardValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.CardId)
            .NotEmpty();

        RuleFor(c => c.TargetListId)
            .NotEmpty();

        RuleFor(c => c.TargetPosition)
            .GreaterThanOrEqualTo(0);
    }
}
