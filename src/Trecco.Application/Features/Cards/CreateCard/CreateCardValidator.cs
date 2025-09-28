namespace Trecco.Application.Features.Cards.CreateCard;

internal sealed class CreateCardValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.ListId)
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
