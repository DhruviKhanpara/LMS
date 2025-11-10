using Microsoft.AspNetCore.Http;

namespace LMS.Common.Helpers;

public static class UserInfoSetup
{
    public static string GetUserEmail(this HttpContext _httpContext)
    {
        return _httpContext?.User?.GetUserEmail() ?? string.Empty;
    }

    public static string GetUserName(this HttpContext _httpContext)
    {
        return _httpContext?.User?.GetUserName() ?? string.Empty;
    }

    public static string GetProfilePhoto(this HttpContext _httpContext)
    {
        return _httpContext?.User?.GetProfilePhoto() ?? string.Empty;
    }

    public static string GetUserId(this HttpContext _httpContext)
    {
        return _httpContext?.User?.GetUserId() ?? string.Empty;
    }

    public static string GetUserRole(this HttpContext _httpContext)
    {
        return _httpContext?.User?.GetUserRole() ?? string.Empty;
    }

    public static string GetOriginBaseURL(this HttpContext _httpContext)
    {
        string? originUrl = _httpContext?.Request.Headers["Origin"];

        if (string.IsNullOrWhiteSpace(originUrl))
            throw new Exception("Error receiving while Return URL from 'Origin' HTTP Header");
        return originUrl;
    }

    public static string GetBaseURL(this HttpContext _httpContext)
    {
        return $"{_httpContext?.Request.Scheme}://{_httpContext?.Request.Host.ToString()}";
    }
}
