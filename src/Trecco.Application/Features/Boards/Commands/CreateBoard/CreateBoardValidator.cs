namespace Trecco.Application.Features.Boards.Commands.CreateBoard;

internal sealed class CreateBoardValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardValidator()
    {
        RuleFor(b => b.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}
