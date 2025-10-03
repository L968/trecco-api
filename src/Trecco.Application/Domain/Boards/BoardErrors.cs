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

    public static Error NotAuthorized =>
        Error.Forbidden(
            "Board.NotAuthorized",
            "User is not allowed to access this board."
        );

    public static Error NoPermission =>
        Error.Forbidden(
            "Board.NoPermission",
            "User does not have permission to perform this action."
        );

    public static Error CannotRemoveOwner =>
        Error.Forbidden(
            "Board.CannotRemoveOwner",
            "The owner cannot remove themselves from the board."
        );

    public static Error CannotRemoveOtherMember =>
        Error.Forbidden(
            "Board.CannotRemoveOtherMember",
            "A member cannot remove other members from the board."
        );
}
