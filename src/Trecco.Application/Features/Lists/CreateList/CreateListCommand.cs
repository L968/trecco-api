namespace Trecco.Application.Features.Lists.CreateList;

public sealed record CreateListCommand(
    Guid BoardId,
    string Name
) : IRequest<Result<CreateListResponse>>;
