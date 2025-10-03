namespace Trecco.Application.Features.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardValidator : AbstractValidator<DeleteBoardCommand>
{
    public DeleteBoardValidator()
    {
        RuleFor(b => b.BoardId)
            .NotEmpty();

        RuleFor(b => b.RequesterId)
            .NotEmpty();
    }
}
