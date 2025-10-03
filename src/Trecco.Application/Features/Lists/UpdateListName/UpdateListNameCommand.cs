namespace Trecco.Application.Features.Lists.UpdateListName;

public sealed record UpdateListNameCommand(
    Guid BoardId,
    Guid ListId,
    string Name,
    Guid RequesterId
) : IRequest<Result>;
