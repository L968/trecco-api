namespace Trecco.Application.Features.Lists.CreateList;

internal sealed class CreateListValidator : AbstractValidator<CreateListCommand>
{
    public CreateListValidator()
    {
        RuleFor(l => l.BoardId).NotEmpty();
        RuleFor(l => l.Name).NotEmpty().MinimumLength(1);
    }
}
