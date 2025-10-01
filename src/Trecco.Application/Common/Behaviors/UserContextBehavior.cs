using Microsoft.Extensions.Primitives;
using Trecco.Application.Common.Authentication;

namespace Trecco.Application.Common.Behaviors;

internal sealed class UserContextBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContext _userContext;

    public UserContextBehavior(
        IHttpContextAccessor httpContextAccessor,
        IUserContext userContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null &&
            httpContext.Request.Headers.TryGetValue("X-User-Id", out StringValues userIdHeader) &&
            Guid.TryParse(userIdHeader, out Guid userId))
        {
            _userContext.SetUserId(userId);
        }

        return await next(cancellationToken);
    }
}
