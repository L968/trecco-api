namespace Trecco.Application.Common.Authentication;

public sealed class UserContext : IUserContext
{
    public Guid? UserId { get; private set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}

