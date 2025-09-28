namespace Trecco.Application.Features.Lists.CreateList;

public sealed record CreateListCommand(
    Guid BoardId,
    string Name,
    Guid RequesterId
) : IRequest<Result>;
