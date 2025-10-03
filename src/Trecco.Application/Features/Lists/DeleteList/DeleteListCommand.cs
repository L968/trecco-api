namespace Trecco.Application.Features.Lists.DeleteList;

public sealed record DeleteListCommand(
    Guid BoardId,
    Guid ListId,
    Guid RequesterId
) : IRequest<Result>;
