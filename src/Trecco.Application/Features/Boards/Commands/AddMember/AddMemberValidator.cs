namespace Trecco.Application.Features.Boards.Commands.AddMember;

internal sealed class AddMemberValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberValidator()
    {
        RuleFor(c => c.BoardId)
            .NotEmpty();

        RuleFor(c => c.MemberId)
            .NotEmpty();

        RuleFor(c => c.RequesterId)
            .NotEmpty();
    }
}
