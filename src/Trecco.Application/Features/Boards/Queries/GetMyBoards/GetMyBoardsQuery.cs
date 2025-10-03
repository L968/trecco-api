namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

public sealed record GetMyBoardsQuery(
    Guid UserId
) : IRequest<IEnumerable<GetMyBoardsResponse>>;
