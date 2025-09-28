namespace Trecco.Application.Features.Cards.DeleteCard;

internal sealed class DeleteCardValidator : AbstractValidator<DeleteCardCommand>
{
    public DeleteCardValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.CardId)
            .NotEmpty();

        RuleFor(c => c.RequesterId)
            .NotEmpty();
    }
}
