namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

public sealed record GetMyBoardsQuery(
    Guid OwnerUserId
) : IRequest<IEnumerable<GetMyBoardsResponse>>;
