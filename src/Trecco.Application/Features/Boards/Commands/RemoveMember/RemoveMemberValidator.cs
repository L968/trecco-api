namespace Trecco.Application.Features.Boards.Commands.RemoveMember;

internal sealed class RemoveMemberValidator : AbstractValidator<RemoveMemberCommand>
{
    public RemoveMemberValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.RequesterId)
            .NotEmpty();
    }
}
