namespace Trecco.Application.Features.Cards.UpdateCard;

internal sealed class UpdateCardValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.CardId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(c => c.Description)
            .MaximumLength(1000);

        RuleFor(c => c.RequesterId)
            .NotEmpty();
    }
}
