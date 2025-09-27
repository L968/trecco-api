namespace Trecco.Application.Domain.Boards;

internal static class BoardErrors
{
    public static Error NotFound(Guid boardId) =>
        Error.NotFound(
            "Board.NotFound",
            $"Board with id \"{boardId}\" was not found."
        );

    public static Error AlreadyMember =>
        Error.Conflict(
            "Board.AlreadyMember",
            $"User is already a member of this board."
        );

    public static Error NotMember =>
        Error.Conflict(
            "Board.NotMember",
            $"User is not a member of this board."
        );
}
