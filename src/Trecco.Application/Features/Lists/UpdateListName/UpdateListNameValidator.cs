namespace Trecco.Application.Features.Lists.UpdateListName;

internal sealed class UpdateListNameValidator : AbstractValidator<UpdateListNameCommand>
{
    public UpdateListNameValidator()
    {
        RuleFor(l => l.BoardId)
            .NotEmpty();

        RuleFor(l => l.ListId)
            .NotEmpty();

        RuleFor(l => l.Name)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(512);

        RuleFor(l => l.RequesterId)
            .NotEmpty();
    }
}
