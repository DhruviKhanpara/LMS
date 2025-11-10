using Hangfire.Dashboard;
using LMS.Core.Enums;

namespace LMS.Presentation.Filter;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public HangfireAuthorizationFilter(params RoleListEnum[] roles)
    {
        _allowedRoles = roles.Select(r => r.ToString()).ToArray();
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            return false;

        var isInRole = _allowedRoles.Any(role => httpContext.User.IsInRole(role));
        var isLocal = httpContext.Connection.RemoteIpAddress?.ToString() == "127.0.0.1";

        return isInRole || isLocal;
    }
}