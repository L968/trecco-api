namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

internal sealed class GetBoardActionLogsValidator : AbstractValidator<GetBoardActionLogsQuery>
{
    public GetBoardActionLogsValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
