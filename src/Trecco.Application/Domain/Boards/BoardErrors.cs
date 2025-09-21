namespace Trecco.Application.Domain.Boards;

internal static class BoardErrors
{
    public static Error BoardAlreadyExists(string boardName) =>
        Error.Conflict("Board.BoardAlreadyExists", $"A board with name \"{boardName}\" already exists.");
}
