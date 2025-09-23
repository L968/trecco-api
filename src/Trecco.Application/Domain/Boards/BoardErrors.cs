namespace Trecco.Application.Domain.Boards;

internal static class BoardErrors
{
    public static Error BoardAlreadyExistsForUser(string boardName) =>
        Error.Conflict(
            "Board.BoardAlreadyExistsForUser",
            $"A board with name \"{boardName}\" already exists for this user."
        );
}
