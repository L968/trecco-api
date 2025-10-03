namespace Trecco.Application.Features.Lists.DeleteList;

internal sealed class DeleteListValidator : AbstractValidator<DeleteListCommand>
{
    public DeleteListValidator()
    {
        RuleFor(l => l.BoardId)
            .NotEmpty();

        RuleFor(l => l.ListId)
            .NotEmpty();

        RuleFor(l => l.RequesterId)
            .NotEmpty();
    }
}
