namespace Trecco.Application.Domain.Lists;

internal static class ListErrors
{
    public static Error NotFound(Guid listId) =>
        Error.NotFound(
            "List.NotFound",
            $"List with id \"{listId}\" was not found."
        );
}
