namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

internal sealed class GetBoardActionLogsValidator : AbstractValidator<GetBoardActionLogsQuery>
{
    public GetBoardActionLogsValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.SearchTerm)
            .MaximumLength(50)
            .Must(term => string.IsNullOrWhiteSpace(term) || !string.IsNullOrWhiteSpace(term.Trim()))
                .WithMessage("Search term cannot be empty or whitespace.");
    }
}
