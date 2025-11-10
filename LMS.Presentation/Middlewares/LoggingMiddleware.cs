using LMS.Common.Logging.Model;
using Serilog.Context;
using LMS.Common.Helpers;

namespace LMS.Presentation.Middlewares;

public class LoggingMiddleware
{
    readonly IConfiguration _configuration;
    readonly RequestDelegate _next;

    public LoggingMiddleware(IConfiguration configuration, RequestDelegate next)
    {
        _configuration = configuration;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var userName = httpContext.GetUserName()
            ?? httpContext.GetUserEmail()
            ?? httpContext.GetUserId()
            ?? System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        LogContext.PushProperty(LoggingProperties.ServerName, Environment.MachineName);
        LogContext.PushProperty(LoggingProperties.UserName, userName);
        LogContext.PushProperty(LoggingProperties.MethodType, httpContext.Request.Method);
        LogContext.PushProperty(LoggingProperties.Origin, httpContext.Request.Headers.Referer);
        LogContext.PushProperty(LoggingProperties.Path, httpContext.Request.Path + httpContext.Request.QueryString);
        LogContext.PushProperty(LoggingProperties.Platform, httpContext.Request.Headers["sec-ch-ua-platform"].ToString());
        LogContext.PushProperty(LoggingProperties.UserAgent, httpContext.Request.Headers["User-Agent"].ToString());

        await _next(httpContext);
    }
}
