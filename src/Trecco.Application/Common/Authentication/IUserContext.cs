namespace Trecco.Application.Common.Authentication;

public interface IUserContext
{
    Guid? UserId { get; }
    void SetUserId(Guid userId);
}

