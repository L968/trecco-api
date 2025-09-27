using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Features.Lists.CreateList;

internal sealed class CreateListHandler(
    IBoardRepository boardRepository,
    ILogger<CreateListHandler> logger
) : IRequestHandler<CreateListCommand, Result<CreateListResponse>>
{
    public async Task<Result<CreateListResponse>> Handle(CreateListCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        List list = board.AddList(request.Name);

        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogDebug("List {ListId} created in Board {BoardId}", list.Id, board.Id);

        return new CreateListResponse(list.Id);
    }
}
