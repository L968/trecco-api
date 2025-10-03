using Trecco.Domain.Results;

namespace Trecco.Domain.Lists;

public static class ListErrors
{
    public static Error NotFound(Guid listId) =>
        Error.NotFound(
            "List.NotFound",
            $"List with id \"{listId}\" was not found."
        );
}
