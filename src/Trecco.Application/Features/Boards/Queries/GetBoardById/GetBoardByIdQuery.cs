namespace Trecco.Application.Features.Boards.Queries.GetBoardById;

public sealed record GetBoardByIdQuery(
    Guid BoardId,
    Guid? UserId
) : IRequest<Result<GetBoardByIdResponse>>;
