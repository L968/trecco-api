namespace Trecco.Application.Features.Boards.Queries.GetBoardById;

public sealed record GetBoardByIdQuery(
    Guid BoardId,
    Guid RequesterId
) : IRequest<Result<GetBoardByIdResponse>>;
