namespace Trecco.Application.Features.Lists.Commands.CreateList;

public sealed record CreateListCommand(
    Guid BoardId,
    string Name
) : IRequest<Result<CreateListResponse>>;
